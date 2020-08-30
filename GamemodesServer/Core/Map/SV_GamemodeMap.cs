using GamemodesServer.Utils;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace GamemodesServer.Core.Map
{
    /// <summary>
    /// Attribute for calling gamemode function on map load
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class GamemodeMapLoadAttribute : Attribute
    {

    }

    /// <summary>
    /// Attribute for calling gamemode function on map load
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class GamemodeMapUnloadAttribute : Attribute
    {

    }

    public abstract class GamemodeMap
    {
        /// <summary>
        /// File name of map
        /// </summary>
        protected string MapFileName { get; set; }

        /// <summary>
        /// Timecycle modifier to use
        /// </summary>
        protected string TimecycMod { get; set; }

        /// <summary>
        /// Extra timecycle modifier to use
        /// </summary>
        protected string TimecycModExtra { get; set; }

        /// <summary>
        /// Time to set
        /// </summary>
        protected TimeSpan Time { get; set; } = new TimeSpan(12, 0, 0);

        /// <summary>
        /// Weather to set
        /// </summary>
        protected string Weather { get; set; } = "EXTRASUNNY";

        /// <summary>
        /// Custom load function
        /// </summary>
        private Func<Task> m_onLoad;

        /// <summary>
        /// Custom unload function
        /// </summary>
        private Func<Task> m_onUnload;

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
                if (methodInfo.GetCustomAttribute(typeof(GamemodeMapLoadAttribute)) != null)
                {
                    Log.WriteLine($"Registering custom OnLoad for gamemode map {methodInfo.DeclaringType.Name}");

                    m_onLoad = createDelegate(methodInfo);
                }
                else if (methodInfo.GetCustomAttribute(typeof(GamemodeMapUnloadAttribute)) != null)
                {
                    Log.WriteLine($"Registering custom OnUnload for gamemode map {methodInfo.DeclaringType.Name}");

                    m_onUnload = createDelegate(methodInfo);
                }
            }
        }

        /// <summary>
        /// Load map function
        /// </summary>
        public async Task Load()
        {
            // Load map file if it exists
            if (MapFileName != null)
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

            // Call custom unload function if available
            if (m_onUnload != null)
            {
                await m_onUnload();
            }
        }
    }
}
