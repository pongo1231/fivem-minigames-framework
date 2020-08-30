using GamemodesServer.Core.Map;
using GamemodesServer.Utils;
using GamemodesShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace GamemodesServer.Core.Gamemode
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

    public abstract class GamemodeScript<MapType> : GamemodeBaseScript where MapType : GamemodeMap
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

        protected bool IsGamemodePreStartRunning { get; private set; } = false;
        protected MapType CurrentMap { get; private set; }

        private Func<Task> m_onPreStart;
        private Func<Task> m_onStart;
        private Func<Task> m_onPreStop;
        private Func<Task> m_onStop;
        private Func<Task> m_onTimerUp;

        private List<GamemodeEventHandler> m_eventHandlers = new List<GamemodeEventHandler>();
        private List<Func<Task>> m_onTickFuncs = new List<Func<Task>>();

        private List<MapType> m_gamemodeMaps = new List<MapType>();

        public GamemodeScript()
        {
            Func<MethodInfo, Func<Task>> createDelegate = (_methodInfo) =>
            {
                return _methodInfo.IsStatic
                    ? (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), _methodInfo)
                    : (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), this, _methodInfo);
            };

            foreach (MethodInfo methodInfo in GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
            {
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
                    Log.WriteLine($"Registering OnTick for gamemode {methodInfo.DeclaringType.Name}");

                    m_onTickFuncs.Add(createDelegate(methodInfo));
                }
            }

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(_type => _type.IsClass && !_type.IsAbstract && _type.IsSubclassOf(typeof(MapType))))
            {
                Log.WriteLine($"Registering map {type.Name} for gamemode {GetType().Name}");

                m_gamemodeMaps.Add((MapType)Activator.CreateInstance(type));
            }

            if (m_gamemodeMaps.Count == 0)
            {
                Log.WriteLine($"!!! GAMEMODE {GetType().Name} HAS NO MAPS REGISTERED !!!");
            }

            GamemodeManager.RegisterGamemode(this);
        }

        public override async Task PreStart()
        {
            IsGamemodePreStartRunning = true;

            CurrentMap = m_gamemodeMaps[new Random().Next(0, m_gamemodeMaps.Count)];

            await CurrentMap.Load();

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

        public override async Task Start()
        {
            IsGamemodePreStartRunning = false;

            if (m_onStart != null)
            {
                await m_onStart();
            }
        }

        public override async Task PreStop()
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

        public override async Task Stop()
        {
            await CurrentMap.Unload();

            EntityPool.ClearEntities();

            if (m_onStop != null)
            {
                await m_onStop();
            }
        }

        public override async void TimerUp()
        {
            if (m_onTimerUp != null)
            {
                await m_onTimerUp();
            }
        }

        public override abstract ETeamType GetWinnerTeam();

        protected void StopGamemode()
        {
            GamemodeManager.StopGamemode();
        }
    }
}
