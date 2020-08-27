using GamemodesShared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace GamemodesServer.Gamemodes
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
    public class GamemodeTimerUpAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GamemodeEventHandlerAttribute : Attribute
    {
        public string EventName { get; private set; }

        public GamemodeEventHandlerAttribute(string _eventName)
        {
            EventName = _eventName;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GamemodeTickAttribute : Attribute
    {

    }

    public abstract class GamemodeScript : GmScript
    {
        private struct GamemodeEventHandler
        {
            public string EventName { get; private set; }
            public Delegate Callback { get; private set; }

            public GamemodeEventHandler(string _eventName, Delegate _callback)
            {
                EventName = _eventName;
                Callback = _callback;
            }
        }

        public string Name { get; protected set; } = "???";
        public string Description { get; protected set; } = "???";
        public string EventName { get; protected set; } = "null";
        public int TimerSeconds { get; protected set; } = 180;

        protected bool IsGamemodePreStartRunning { get; private set; } = false;

        private Func<Task> m_onPreStart;
        private Func<Task> m_onStart;
        private Func<Task> m_onPreStop;
        private Func<Task> m_onStop;
        private Func<Task> m_onTimerUp;

        private List<GamemodeEventHandler> m_eventHandlers = new List<GamemodeEventHandler>();
        private List<Func<Task>> m_onTickFuncs = new List<Func<Task>>();

        public GamemodeScript()
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
                else if (methodInfo.GetCustomAttribute(typeof(GamemodeTimerUpAttribute)) != null)
                {
                    Debug.WriteLine($"Registering custom OnTimerUp for gamemode {methodInfo.DeclaringType.Name}");

                    m_onTimerUp = createDelegate(methodInfo);
                }
                else if (methodInfo.GetCustomAttribute(typeof(GamemodeEventHandlerAttribute)) != null)
                {
                    Debug.WriteLine($"Registering EventHandler for gamemode {methodInfo.DeclaringType.Name}");

                    string eventName = ((GamemodeEventHandlerAttribute)methodInfo.GetCustomAttribute(typeof(GamemodeEventHandlerAttribute))).EventName;

                    Type[] parameters = methodInfo.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
                    Type actionType = Expression.GetDelegateType(parameters.Concat(new[] { typeof(void) }).ToArray());

                    Delegate callback = methodInfo.IsStatic
                        ? Delegate.CreateDelegate(actionType, methodInfo)
                        : Delegate.CreateDelegate(actionType, this, methodInfo);

                    m_eventHandlers.Add(new GamemodeEventHandler(eventName, callback));
                }
                else if (methodInfo.GetCustomAttribute(typeof(GamemodeTickAttribute)) != null)
                {
                    Debug.WriteLine($"Registering OnTick for gamemode {methodInfo.DeclaringType.Name}");

                    m_onTickFuncs.Add(createDelegate(methodInfo));
                }
            }

            GamemodeManager.RegisterGamemode(this);
        }

        public async Task OnPreStart()
        {
            IsGamemodePreStartRunning = true;

            if (m_onPreStart != null)
            {
                await m_onPreStart();
            }

            foreach (GamemodeEventHandler eventHandler in m_eventHandlers)
            {
                EventHandlers[eventHandler.EventName] += eventHandler.Callback;
            }

            foreach (Func<Task> onTickFunc in m_onTickFuncs)
            {
                Tick += onTickFunc;
            }
        }

        public async Task OnStart()
        {
            IsGamemodePreStartRunning = false;

            if (m_onStart != null)
            {
                await m_onStart();
            }
        }

        public async Task OnPreStop()
        {
            foreach (GamemodeEventHandler eventHandler in m_eventHandlers)
            {
                EventHandlers[eventHandler.EventName] -= eventHandler.Callback;
            }

            foreach (Func<Task> onTickFunc in m_onTickFuncs)
            {
                Tick -= onTickFunc;
            }

            if (m_onPreStop != null)
            {
                await m_onPreStop();
            }
        }

        public async Task OnStop()
        {
            if (m_onStop != null)
            {
                await m_onStop();
            }
        }

        public async void OnTimerUp()
        {
            if (m_onTimerUp != null)
            {
                await m_onTimerUp();
            }
        }

        public abstract EPlayerTeamType GetWinnerTeam();

        protected void Stop()
        {
            GamemodeManager.StopGamemode();
        }
    }
}
