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

            foreach (MethodInfo methodInfo in GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
            {
                if (methodInfo.GetCustomAttribute(typeof(GamemodePreStartAttribute)) != null)
                {
                    Debug.WriteLine($"Registering OnPreStart for gamemode {methodInfo.DeclaringType.Name}");

                    m_onPreStart = createDelegate(methodInfo);

                    EventHandlers[$"gamemodes:cl_sv_{_eventName}_prestart"] += new Action(OnPreStart);
                }
                else if (methodInfo.GetCustomAttribute(typeof(GamemodeStartAttribute)) != null)
                {
                    Debug.WriteLine($"Registering OnStart for gamemode {methodInfo.DeclaringType.Name}");

                    m_onStart = createDelegate(methodInfo);

                    EventHandlers[$"gamemodes:cl_sv_{_eventName}_start"] += new Action(OnStart);
                }
                else if (methodInfo.GetCustomAttribute(typeof(GamemodePreStopAttribute)) != null)
                {
                    Debug.WriteLine($"Registering OnPreStop for gamemode {methodInfo.DeclaringType.Name}");

                    m_onPreStop = createDelegate(methodInfo);

                    EventHandlers[$"gamemodes:cl_sv_{_eventName}_prestop"] += new Action(OnPreStop);
                }
                else if (methodInfo.GetCustomAttribute(typeof(GamemodeStopAttribute)) != null)
                {
                    Debug.WriteLine($"Registering OnStop for gamemode {methodInfo.DeclaringType.Name}");

                    m_onStop = createDelegate(methodInfo);

                    EventHandlers[$"gamemodes:cl_sv_{_eventName}_stop"] += new Action(OnStop);
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

            API.SetPlayerControl(Game.Player.Handle, false, 1 << 8);

            foreach (Func<Task> onTickFunc in m_onTickFuncs)
            {
                Tick += onTickFunc;
            }

            await m_onPreStart?.Invoke();

            TriggerServerEvent("gamemodes:sv_cl_prestartedgamemode");
        }

        private async void OnStart()
        {
            IsPreStartRunning = false;

            API.SetPlayerControl(Game.Player.Handle, true, 0);

            MusicManager.Play();

            Screen.Effects.Start(ScreenEffect.MpCelebWinOut);

            await m_onStart?.Invoke();

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

            await m_onPreStop?.Invoke();

            TriggerServerEvent("gamemodes:sv_cl_prestoppedgamemode");
        }

        private async void OnStop()
        {
            API.ClearTimecycleModifier();
            API.ClearExtraTimecycleModifier();

            await m_onStop?.Invoke();

            TriggerServerEvent("gamemodes:sv_cl_stoppedgamemode");
        }
    }
}
