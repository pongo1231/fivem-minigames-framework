using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace GamemodesClient.Gamemodes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GamemodePreStartAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GamemodeStartAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GamemodePreStopAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GamemodeStopAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GamemodeTickAttribute : Attribute
    {

    }

    public abstract class GamemodeScript : BaseScript
    {
        protected bool IsGamemodeRunning { get; private set; } = false;
        protected bool IsPreStartRunning { get; private set; } = false;

        private Func<Task> m_onPreStart;
        private Func<Task> m_onStart;
        private Func<Task> m_onPreStop;
        private Func<Task> m_onStop;

        private List<Func<Task>> m_onTickFuncs = new List<Func<Task>>();

        public GamemodeScript(string _eventName)
        {
            Func<MethodInfo, Func<Task>> createDelegate = (MethodInfo _methodInfo) =>
            {
                return _methodInfo.IsStatic
                    ? (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), _methodInfo)
                    : (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), this, _methodInfo);
            };

            EventHandlers[$"gamemodes:cl_sv_{_eventName}_prestart"] += new Action(OnPreStart);
            EventHandlers[$"gamemodes:cl_sv_{_eventName}_start"] += new Action(OnStart);
            EventHandlers[$"gamemodes:cl_sv_{_eventName}_prestop"] += new Action(OnPreStop);
            EventHandlers[$"gamemodes:cl_sv_{_eventName}_stop"] += new Action(OnStop);

            foreach (MethodInfo methodInfo in GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
            {
                if (methodInfo.GetCustomAttribute(typeof(GamemodePreStartAttribute)) != null)
                {
                    Debug.WriteLine($"Registering custom OnPreStart for gamemode {methodInfo.DeclaringType.Name}");

                    m_onPreStart = createDelegate(methodInfo);
                }
                else if (methodInfo.GetCustomAttribute(typeof(GamemodeStartAttribute)) != null)
                {
                    Debug.WriteLine($"Registering custom OnStart for gamemode {methodInfo.DeclaringType.Name}");

                    m_onStart = createDelegate(methodInfo);
                }
                else if (methodInfo.GetCustomAttribute(typeof(GamemodePreStopAttribute)) != null)
                {
                    Debug.WriteLine($"Registering custom OnPreStop for gamemode {methodInfo.DeclaringType.Name}");

                    m_onPreStop = createDelegate(methodInfo);
                }
                else if (methodInfo.GetCustomAttribute(typeof(GamemodeStopAttribute)) != null)
                {
                    Debug.WriteLine($"Registering custom OnStop for gamemode {methodInfo.DeclaringType.Name}");

                    m_onStop = createDelegate(methodInfo);
                }
                else if (methodInfo.GetCustomAttribute(typeof(GamemodeTickAttribute)) != null)
                {
                    Debug.WriteLine($"Registering OnTick for gamemode {methodInfo.DeclaringType.Name}");

                    m_onTickFuncs.Add(createDelegate(methodInfo));
                }
            }
        }

        private async void OnPreStart()
        {
            IsGamemodeRunning = true;
            IsPreStartRunning = true;

            PlayerControlManager.HasControl = false;

            foreach (Func<Task> onTickFunc in m_onTickFuncs)
            {
                Tick += onTickFunc;
            }

            if (m_onPreStart != null)
            {
                await m_onPreStart();
            }

            TriggerServerEvent("gamemodes:sv_cl_prestartedgamemode");
        }

        private async void OnStart()
        {
            IsPreStartRunning = false;

            PlayerControlManager.HasControl = true;

            MusicManager.Play();

            Screen.Effects.Start(ScreenEffect.MpCelebWinOut);

            if (m_onStart != null)
            {
                await m_onStart();
            }

            TriggerServerEvent("gamemodes:sv_cl_startedgamemode");
        }

        private async void OnPreStop()
        {
            IsGamemodeRunning = false;

            foreach (Func<Task> onTickFunc in m_onTickFuncs)
            {
                Tick -= onTickFunc;
            }

            MusicManager.Stop();

            if (m_onPreStop != null)
            {
                await m_onPreStop();
            }

            TriggerServerEvent("gamemodes:sv_cl_prestoppedgamemode");
        }

        private async void OnStop()
        {
            API.ClearTimecycleModifier();
            API.ClearExtraTimecycleModifier();

            if (m_onStop != null)
            {
                await m_onStop();
            }

            TriggerServerEvent("gamemodes:sv_cl_stoppedgamemode");
        }
    }
}
