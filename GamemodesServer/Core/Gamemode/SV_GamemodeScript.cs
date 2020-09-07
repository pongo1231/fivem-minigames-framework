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
    /// Gamemode script class
    /// </summary>
    /// <typeparam name="MapType">Type of map class that corresponds to this gamemode</typeparam>
    public abstract class GamemodeScript<MapType> : GamemodeBaseScript where MapType : GamemodeMap
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
        protected MapType CurrentMap { get; private set; }

        /// <summary>
        /// Custom prestart function
        /// </summary>
        private Func<Task> m_onPreStart;

        /// <summary>
        /// Custom start function
        /// </summary>
        private Func<Task> m_onStart;

        /// <summary>
        /// Custom prestop function
        /// </summary>
        private Func<Task> m_onPreStop;

        /// <summary>
        /// Custom stop function
        /// </summary>
        private Func<Task> m_onStop;

        /// <summary>
        /// Custom timer up function
        /// </summary>
        private Func<Task> m_onTimerUp;

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
            // Create delegate helper
            Func<MethodInfo, Func<Task>> createDelegate = (_methodInfo) =>
            {
                return _methodInfo.IsStatic
                    ? (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), _methodInfo)
                    : (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), this, _methodInfo);
            };

            // Iterate through all functions of child (and all inherited) class(es) via reflection
            foreach (MethodInfo methodInfo in GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
            {
                /* Check for custom attributes and register methods containing them correspondely */

                if (methodInfo.GetCustomAttribute(typeof(GamemodePreStartAttribute)) != null)
                {
                    Log.WriteLine($"Registering custom OnPreStart for gamemode {methodInfo.DeclaringType.Name}");

                    m_onPreStart = createDelegate(methodInfo);
                }
                else if (methodInfo.GetCustomAttribute(typeof(GamemodeStartAttribute)) != null)
                {
                    Log.WriteLine($"Registering custom OnStart for gamemode {methodInfo.DeclaringType.Name}");

                    m_onStart = createDelegate(methodInfo);
                }
                else if (methodInfo.GetCustomAttribute(typeof(GamemodePreStopAttribute)) != null)
                {
                    Log.WriteLine($"Registering custom OnPreStop for gamemode {methodInfo.DeclaringType.Name}");

                    m_onPreStop = createDelegate(methodInfo);
                }
                else if (methodInfo.GetCustomAttribute(typeof(GamemodeStopAttribute)) != null)
                {
                    Log.WriteLine($"Registering custom OnStop for gamemode {methodInfo.DeclaringType.Name}");

                    m_onStop = createDelegate(methodInfo);
                }
                else if (methodInfo.GetCustomAttribute(typeof(GamemodeTimerUpAttribute)) != null)
                {
                    Log.WriteLine($"Registering custom OnTimerUp for gamemode {methodInfo.DeclaringType.Name}");

                    m_onTimerUp = createDelegate(methodInfo);
                }
                else if (methodInfo.GetCustomAttribute(typeof(GamemodeEventHandlerAttribute)) != null)
                {
                    Log.WriteLine($"Registering EventHandler for gamemode {methodInfo.DeclaringType.Name}");

                    // Get event name
                    string eventName = ((GamemodeEventHandlerAttribute)methodInfo.GetCustomAttribute(typeof(GamemodeEventHandlerAttribute))).EventName;

                    // Get parameters
                    Type[] parameters = methodInfo.GetParameters().Select(parameter => parameter.ParameterType).ToArray();

                    // Build type for callback
                    Type actionType = Expression.GetDelegateType(parameters.Concat(new[] { typeof(void) }).ToArray());

                    // Create callback delegate
                    Delegate callback = methodInfo.IsStatic
                        ? Delegate.CreateDelegate(actionType, methodInfo)
                        : Delegate.CreateDelegate(actionType, this, methodInfo);

                    // Add to list of event handlers
                    m_eventHandlers.Add(new GamemodeEventHandler(eventName, callback));
                }
                else if (methodInfo.GetCustomAttribute(typeof(GamemodeTickAttribute)) != null)
                {
                    Log.WriteLine($"Registering OnTick for gamemode {methodInfo.DeclaringType.Name}");

                    // Add to list of tick delegates
                    m_onTickFuncs.Add(createDelegate(methodInfo));
                }
            }

            // Go through all existing types to search for maps mapped to this gamemode
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(_type => _type.IsClass && !_type.IsAbstract && _type.IsSubclassOf(typeof(MapType))))
            {
                Log.WriteLine($"Registering map {type.Name} for gamemode {GetType().Name}");

                // Create instance of map
                MapType mapType = (MapType)Activator.CreateInstance(type);

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

            // Choose a random map
            CurrentMap = m_gamemodeMaps[RandomUtils.RandomInt(0, m_gamemodeMaps.Count)];

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
            foreach (GamemodeEventHandler eventHandler in m_eventHandlers)
            {
                EventHandlers[eventHandler.EventName] += eventHandler.Callback;
            }

            // Register all tick functions
            foreach (Func<Task> onTickFunc in m_onTickFuncs)
            {
                Tick += onTickFunc;
            }
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
            foreach (GamemodeEventHandler eventHandler in m_eventHandlers)
            {
                EventHandlers[eventHandler.EventName] -= eventHandler.Callback;
            }

            // Unregister all tick functions
            foreach (Func<Task> onTickFunc in m_onTickFuncs)
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
            // Unload map
            await CurrentMap.Unload();

            // Clear entity pool
            EntityPool.ClearEntities();

            // Call custom stop function if available
            if (m_onStop != null)
            {
                await m_onStop();
            }
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
