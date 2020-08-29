using GamemodesServer.Utils;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace GamemodesServer.Core.Map
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GamemodeMapLoadAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GamemodeMapUnloadAttribute : Attribute
    {

    }

    public abstract class GamemodeMap
    {
        protected string MapFileName { get; set; }
        protected string TimecycMod { get; set; }
        protected string TimecycModExtra { get; set; }
        protected TimeSpan Time { get; set; } = new TimeSpan(12, 0, 0);
        protected string Weather { get; set; } = "EXTRASUNNY";

        private Func<Task> m_onLoad;
        private Func<Task> m_onUnload;

        public GamemodeMap()
        {
            Func<MethodInfo, Func<Task>> createDelegate = (_methodInfo) =>
            {
                return _methodInfo.IsStatic
                    ? (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), _methodInfo)
                    : (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), this, _methodInfo);
            };

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

        public async Task Load()
        {
            if (MapFileName != null)
            {
                await MapLoader.LoadMap($"maps/{MapFileName}");
            }

            TimecycModManager.SetTimecycModifiers(TimecycMod, TimecycModExtra);

            TimeWeatherManager.SetTime(Time.Hours, Time.Minutes, Time.Seconds);
            TimeWeatherManager.SetWeather(Weather);

            if (m_onLoad != null)
            {
                await m_onLoad();
            }
        }

        public async Task Unload()
        {
            MapLoader.ClearMap();

            TimecycModManager.ClearTimecycModifiers();

            if (m_onUnload != null)
            {
                await m_onUnload();
            }
        }
    }
}
