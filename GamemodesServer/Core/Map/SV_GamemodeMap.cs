using GamemodesServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace GamemodesServer.Core.Map
{
    /// <summary>
    /// Gamemode map base script
    /// </summary>
    public abstract class GamemodeMap : GmScript
    {
        /// <summary>
        /// Attribute for calling map function on map load
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        protected class MapLoadAttribute : Attribute
        {

        }

        /// <summary>
        /// Attribute for calling map function on map load
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        protected class MapUnloadAttribute : Attribute
        {

        }

        /// <summary>
        /// Attribute for registering event while map is running
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        protected class MapEventHandlerAttribute : Attribute
        {
            public string EventName { get; private set; }

            public MapEventHandlerAttribute(string _eventName)
            {
                EventName = _eventName;
            }
        }

        /// <summary>
        /// Attribute for registering tick function while map is running
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        protected class MapTickAttribute : Attribute
        {

        }

        /// <summary>
        /// Map event handler class
        /// </summary>
        private struct MapEventHandler
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
            public MapEventHandler(string _eventName, Delegate _callback)
            {
                EventName = _eventName;
                Callback = _callback;
            }
        }

        /// <summary>
        /// Whether this map should be not a choice when selecting a new one
        /// </summary>
        public bool ExcludeFromChoicesList = false;

        /// <summary>
        /// Whether prestart is currently running
        /// </summary>
        public bool IsGamemodePreStartRunning = false;

        /// <summary>
        /// Name of map
        /// </summary>
        public string MapName { get; protected set; }

        /// <summary>
        /// Height at which stuff should respawn
        /// </summary>
        public float FallOffHeight { get; protected set; } = float.MinValue;

        /// <summary>
        /// File name of map
        /// </summary>
        protected string MapFileName;

        /// <summary>
        /// Timecycle modifier to use
        /// </summary>
        protected string TimecycMod;

        /// <summary>
        /// Extra timecycle modifier to use
        /// </summary>
        protected string TimecycModExtra;

        /// <summary>
        /// Time to set
        /// </summary>
        protected TimeSpan Time = new TimeSpan(12, 0, 0);

        /// <summary>
        /// Weather to set
        /// </summary>
        protected string Weather = "EXTRASUNNY";

        /// <summary>
        /// Custom load function
        /// </summary>
        private Func<Task> m_onLoad;

        /// <summary>
        /// Custom unload function
        /// </summary>
        private Func<Task> m_onUnload;

        /// <summary>
        /// List of event handlers
        /// </summary>
        private List<MapEventHandler> m_eventHandlers = new List<MapEventHandler>();

        /// <summary>
        /// List of tick functions
        /// </summary>
        private List<Func<Task>> m_onTickFuncs = new List<Func<Task>>();

        public GamemodeMap()
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
                if (methodInfo.GetCustomAttribute(typeof(MapLoadAttribute)) != null)
                {
                    Log.WriteLine($"Registering custom OnLoad for map {methodInfo.DeclaringType.Name}");

                    m_onLoad = createDelegate(methodInfo);
                }
                else if (methodInfo.GetCustomAttribute(typeof(MapUnloadAttribute)) != null)
                {
                    Log.WriteLine($"Registering custom OnUnload for map {methodInfo.DeclaringType.Name}");

                    m_onUnload = createDelegate(methodInfo);
                }
                else if (methodInfo.GetCustomAttribute(typeof(MapEventHandlerAttribute)) != null)
                {
                    Log.WriteLine($"Registering EventHandler for map {methodInfo.DeclaringType.Name}");

                    // Get event name
                    string eventName = ((MapEventHandlerAttribute)methodInfo.GetCustomAttribute(typeof(MapEventHandlerAttribute))).EventName;

                    // Get parameters
                    Type[] parameters = methodInfo.GetParameters().Select(parameter => parameter.ParameterType).ToArray();

                    // Build type for callback
                    Type actionType = Expression.GetDelegateType(parameters.Concat(new[] { typeof(void) }).ToArray());

                    // Create callback delegate
                    Delegate callback = methodInfo.IsStatic
                        ? Delegate.CreateDelegate(actionType, methodInfo)
                        : Delegate.CreateDelegate(actionType, this, methodInfo);

                    // Add to list of event handlers
                    m_eventHandlers.Add(new MapEventHandler(eventName, callback));
                }
                else if (methodInfo.GetCustomAttribute(typeof(MapTickAttribute)) != null)
                {
                    Log.WriteLine($"Registering OnTick for map {methodInfo.DeclaringType.Name}");

                    // Add to list of tick delegates
                    m_onTickFuncs.Add(createDelegate(methodInfo));
                }
            }
        }

        /// <summary>
        /// Load map function
        /// </summary>
        public async Task Load()
        {
            // Load map file if it exists
            if (MapFileName == null)
            {
                Log.WriteLine($"!!! Map {GetType().Name} has no MapFileName configured !!!");
            }
            else
            {
                // Start in maps directory
                await MapLoader.LoadMap($"maps/{MapFileName}");
            }

            // Set timecycle modifiers
            TimecycModManager.SetTimecycModifiers(TimecycMod, TimecycModExtra);

            // Set time
            TimeWeatherManager.SetTime(Time.Hours, Time.Minutes, Time.Seconds);

            // Set weather
            TimeWeatherManager.SetWeather(Weather);

            // Call custom load function if available
            if (m_onLoad != null)
            {
                await m_onLoad();
            }

            // Register all event handlers
            foreach (MapEventHandler eventHandler in m_eventHandlers)
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
        /// Unload function
        /// </summary>
        public async Task Unload()
        {
            // Clear map
            MapLoader.ClearMap();

            // Clear timecycle modifiers
            TimecycModManager.ClearTimecycModifiers();

            // Unregister all event handlers
            foreach (MapEventHandler eventHandler in m_eventHandlers)
            {
                EventHandlers[eventHandler.EventName] -= eventHandler.Callback;
            }

            // Unregister all tick functions
            foreach (Func<Task> onTickFunc in m_onTickFuncs)
            {
                Tick -= onTickFunc;
            }

            // Call custom unload function if available
            if (m_onUnload != null)
            {
                await m_onUnload();
            }
        }
    }
}
