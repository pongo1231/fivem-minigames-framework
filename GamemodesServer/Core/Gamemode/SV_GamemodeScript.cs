﻿using CitizenFX.Core.Native;
using GamemodesServer.Core.Map;
using GamemodesServer.Utils;
using GamemodesShared;
using GamemodesShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace GamemodesServer.Core.Gamemode
{
    /// <summary>
    /// Gamemode script
    /// </summary>
    /// <typeparam name="MapType">Type of map class that corresponds to this gamemode</typeparam>
    public abstract class GamemodeScript<MapType> : GamemodeBaseScript
        where MapType : GamemodeBaseMap
    {
        /// <summary>
        /// Gamemode event handler class
        /// </summary>
        private struct GamemodeEventHandler
        {
            /// <summary>
            /// Name of event
            /// </summary>
            public string EventName { get; private set; }

            /// <summary>
            /// Callback for event
            /// </summary>
            public Delegate Callback { get; private set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="_eventName">Name of event</param>
            /// <param name="_callback">Callback for event</param>
            public GamemodeEventHandler(string _eventName, Delegate _callback)
            {
                EventName = _eventName;
                Callback = _callback;
            }
        }

        /// <summary>
        /// Whether prestart is currently running
        /// </summary>
        protected bool IsGamemodePreStartRunning { get; private set; } = false;

        /// <summary>
        /// Current map of gamemode
        /// </summary>
        protected MapType CurrentMap { get; private set; } = null;

        /// <summary>
        /// Event for custom prestart functions
        /// </summary>
        private event Func<Task> m_onPreStart = null;

        /// <summary>
        /// Event for custom start functions
        /// </summary>
        private event Func<Task> m_onStart = null;

        /// <summary>
        /// Event for custom prestop functions
        /// </summary>
        private event Func<Task> m_onPreStop = null;

        /// <summary>
        /// Event for custom stop functions
        /// </summary>
        private event Func<Task> m_onStop = null;

        /// <summary>
        /// Event for custom timer up functions
        /// </summary>
        private event Func<Task> m_onTimerUp = null;

        /// <summary>
        /// List of event handlers
        /// </summary>
        private List<GamemodeEventHandler> m_eventHandlers = new List<GamemodeEventHandler>();

        /// <summary>
        /// List of tick functions
        /// </summary>
        private List<Func<Task>> m_onTickFuncs = new List<Func<Task>>();

        /// <summary>
        /// List of maps registered for this gamemode
        /// </summary>
        private List<MapType> m_gamemodeMaps = new List<MapType>();

        /// <summary>
        /// Constructor
        /// </summary>
        public GamemodeScript()
        {
            /* Register methods with corresponding attributes */

            ReflectionUtils
                .GetAllMethodsWithAttributeForClass<Func<Task>, GamemodePreStartAttribute>(this,
                    ref m_onPreStart);

            ReflectionUtils
                .GetAllMethodsWithAttributeForClass<Func<Task>, GamemodeStartAttribute>(this,
                    ref m_onStart);

            ReflectionUtils
                .GetAllMethodsWithAttributeForClass<Func<Task>, GamemodePreStopAttribute>(this,
                    ref m_onPreStop);

            ReflectionUtils
                .GetAllMethodsWithAttributeForClass<Func<Task>, GamemodeStopAttribute>(this,
                    ref m_onStop);

            ReflectionUtils
                .GetAllMethodsWithAttributeForClass<Func<Task>, GamemodeTimerUpAttribute>(this,
                    ref m_onTimerUp);

            m_onTickFuncs.AddRange(ReflectionUtils
                .GetAllMethodsWithAttributeForClass<Func<Task>, GamemodeTickAttribute>(this));

            foreach (MethodInfo methodInfo in GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static
                    | BindingFlags.Instance)
                .Where(_methodInfo =>
                    _methodInfo.GetCustomAttribute(typeof(GamemodeEventHandlerAttribute)) != null))
            {
                // Get event name
                var eventName = ((GamemodeEventHandlerAttribute)methodInfo
                    .GetCustomAttribute(typeof(GamemodeEventHandlerAttribute))).EventName;

                // Get parameters
                var parameters = methodInfo.GetParameters()
                    .Select(parameter => parameter.ParameterType).ToArray();

                // Build type for callback
                var actionType = Expression
                    .GetDelegateType(parameters.Concat(new[] { typeof(void) }).ToArray());

                // Create callback delegate
                var callback = methodInfo.IsStatic
                    ? Delegate.CreateDelegate(actionType, methodInfo)
                    : Delegate.CreateDelegate(actionType, this, methodInfo);

                // Add to list of event handlers
                m_eventHandlers.Add(new GamemodeEventHandler(eventName, callback));
            }

            // Go through all existing types to search for maps mapped to this gamemode
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes()
                .Where(_type => _type.IsClass && !_type.IsAbstract
                    && _type.IsSubclassOf(typeof(MapType))))
            {
                // Create instance of map
                var mapType = (MapType)Activator.CreateInstance(type);

                // Add to list of registered maps
                m_gamemodeMaps.Add(mapType);

                // Register map script
                RegisterScript(mapType);
            }

            // Only register gamemode if maps exist for gamemode
            if (m_gamemodeMaps.Count == 0)
            {
                Log.WriteLine($"!!! GAMEMODE {GetType().Name} HAS NO MAPS REGISTERED !!!");
            }
            else
            {
                // Register gamemode
                GamemodeManager.RegisterGamemode(this);
            }
        }

        /// <summary>
        /// Pre start function
        /// </summary>
        public override async Task PreStart()
        {
            // Set gamemode as prestarting
            IsGamemodePreStartRunning = true;

            // Check if there's a forced map set as convar
            var forcedMapName = API.GetConvar($"gamemodes_{EventName}_forced_map", string.Empty);
            var forcedMap = m_gamemodeMaps.Find(_map => _map.MapName == forcedMapName);

            // Choose map to load
            if (forcedMap != null)
            {
                // Set gamemode to forced gamemode
                CurrentMap = forcedMap;
            }
            else
            {
                // Notify if forced map couldn't be found
                if (forcedMapName != string.Empty)
                {
                    Log.WriteLine(
                        $"Couldn't find forced map {forcedMapName} for gamemode {EventName}!");
                }

                // Store all except previous maps
                var mapChoices = m_gamemodeMaps
                    .Where(_map => !_map.ExcludeFromChoicesList).ToArray();

                // Select random map
                CurrentMap = mapChoices[RandomUtils.RandomInt(0, mapChoices.Length)];

                // Reset exclude status for all maps if this map is the last non-excluded one
                if (m_gamemodeMaps.Where(_map => !_map.ExcludeFromChoicesList).Count() <= 1)
                {
                    foreach (var map in m_gamemodeMaps)
                    {
                        map.ExcludeFromChoicesList = false;
                    }
                }

                // Exclude this map from list until all other maps have been chosen
                // (if there are more than 1 registered maps)
                if (m_gamemodeMaps.Count > 1)
                {
                    CurrentMap.ExcludeFromChoicesList = true;
                }
            }

            // Set gamemode as prestarting to map too
            CurrentMap.IsGamemodePreStartRunning = true;

            // Load map
            await CurrentMap.Load();

            // Call custom prestart function if available
            if (m_onPreStart != null)
            {
                await m_onPreStart();
            }

            // Register all event handlers
            foreach (var eventHandler in m_eventHandlers)
            {
                EventHandlers[eventHandler.EventName] += eventHandler.Callback;
            }

            // Register all tick functions
            foreach (var onTickFunc in m_onTickFuncs)
            {
                Tick += onTickFunc;
            }

            Log.WriteLine($"Loaded map {CurrentMap.GetType().Name} for gamemode {EventName}!");
        }

        /// <summary>
        /// Start function
        /// </summary>
        public override async Task Start()
        {
            // Set gamemode as not prestarting anymore
            IsGamemodePreStartRunning = false;

            // Set gamemode as not prestarting to map too
            CurrentMap.IsGamemodePreStartRunning = false;

            // Call custom start function if available
            if (m_onStart != null)
            {
                await m_onStart();
            }
        }

        /// <summary>
        /// Stop function
        /// </summary>
        public override async Task PreStop()
        {
            // Unregister all event handlers
            foreach (var eventHandler in m_eventHandlers)
            {
                EventHandlers[eventHandler.EventName] -= eventHandler.Callback;
            }

            // Unregister all tick functions
            foreach (var onTickFunc in m_onTickFuncs)
            {
                Tick -= onTickFunc;
            }

            // Call custom prestop function if available
            if (m_onPreStop != null)
            {
                await m_onPreStop();
            }
        }

        /// <summary>
        /// Stop function
        /// </summary>
        public override async Task Stop()
        {
            // Reset scores
            ScoreManager.ResetScores();

            // Unload map
            await CurrentMap.Unload();

            // Clear entity pool
            EntityPool.ClearEntities();

            // Call custom stop function if available
            if (m_onStop != null)
            {
                await m_onStop();
            }

            Log.WriteLine($"Unloaded map {CurrentMap.GetType().Name} for gamemode {EventName}!");
        }

        /// <summary>
        /// Timer up function
        /// </summary>
        public override async void TimerUp()
        {
            // Call custom timer up function if available
            if (m_onTimerUp != null)
            {
                await m_onTimerUp();
            }
        }

        /// <summary>
        /// Abstract get winner team function
        /// </summary>
        /// <returns>Winner team</returns>
        public override abstract ETeamType GetWinnerTeam();

        /// <summary>
        /// Stop gamemode
        /// </summary>
        protected void StopGamemode()
        {
            GamemodeManager.StopGamemode();
        }
    }
}
