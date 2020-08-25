using CitizenFX.Core;
using GamemodesServer.Gamemodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GamemodesServer
{
    public class GamemodeManager : GmScript
    {
        private class WrapperTickFunc
        {
            public WrapperTickFunc(Type _type, MethodInfo _methodInfo)
            {
                Type = _type;
                MethodInfo = _methodInfo;
            }

            public async Task Func()
            {
                if (TargetFunc != null)
                {
                    await TargetFunc();
                }
            }

            public Type Type { get; private set; }
            public MethodInfo MethodInfo { get; private set; }
            public Func<Task> TargetFunc;
        }

        private List<Player> m_gamemodePlayers = new List<Player>();

        private static List<GamemodeScript> s_registeredGamemodes = new List<GamemodeScript>();
        private static GamemodeScript s_curGamemode = null;
        private static bool s_stopGamemode = false;

        private Random m_random = new Random();

        private List<WrapperTickFunc> m_registeredWrapperTickFuncs = new List<WrapperTickFunc>();

        private bool m_awaitingGamemodeStop = false;

        public GamemodeManager()
        {
            PlayerDropped += OnPlayerDropped;
            
            foreach (MethodInfo methodInfo in Assembly.GetExecutingAssembly().GetTypes()
                .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)).Where(method => method.GetCustomAttribute(typeof(GamemodeScript.GamemodeTick)) != null))
            {
                WrapperTickFunc wrapperTickFunc = new WrapperTickFunc(methodInfo.DeclaringType, methodInfo);

                Tick += wrapperTickFunc.Func;

                m_registeredWrapperTickFuncs.Add(wrapperTickFunc);
            }
        }

        private void OnPlayerDropped(Player _player)
        {
            m_gamemodePlayers.Remove(_player);
        }

        [Tick]
        public async Task OnGamemodeHandleTick()
        {
            if (PlayerLoadStateManager.GetLoadedInPlayers().Length == 0)
            {
                if (s_curGamemode != null)
                {
                    _StopGamemode();
                }

                return;
            }

            if (s_curGamemode == null)
            {
                m_gamemodePlayers.Clear();

                TeamManager.EnableTeams();
                while (!TeamManager.TeamsLoaded)
                {
                    await Delay(0);
                }

                s_curGamemode = s_registeredGamemodes[m_random.Next(0, s_registeredGamemodes.Count)];

                await s_curGamemode.OnStart();

                foreach (WrapperTickFunc wrapperTickFunc in m_registeredWrapperTickFuncs.Where(_wrapperTickFunc => _wrapperTickFunc.Type == s_curGamemode.GetType()))
                {
                    Debug.WriteLine($"Registering gamemode tick function {wrapperTickFunc.MethodInfo.Name} for {wrapperTickFunc.Type.Name}");

                    wrapperTickFunc.TargetFunc = (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), s_curGamemode, wrapperTickFunc.MethodInfo);
                }

                TimerManager.SetTimer(s_curGamemode.TimerSeconds);
            }
            else if (s_stopGamemode)
            {
                _StopGamemode();
            }
            else
            {
                foreach (Player player in PlayerLoadStateManager.GetLoadedInPlayers())
                {
                    if (!m_gamemodePlayers.Contains(player))
                    {
                        m_gamemodePlayers.Add(player);

                        _ = PlayerResponseAwaiter.AwaitResponse(player, $"gamemodes:cl_sv_{s_curGamemode.EventName}_start", "gamemodes:sv_cl_startedgamemode");
                    }
                }

                if (TimerManager.HasTimerRunOut() && !m_awaitingGamemodeStop)
                {
                    m_awaitingGamemodeStop = true;

                    s_curGamemode.OnTimerUp();
                }
            }

            await Task.FromResult(0);
        }

        private void _StopGamemode()
        {
            s_stopGamemode = false;
            m_awaitingGamemodeStop = false;

            foreach (WrapperTickFunc wrapperTickFunc in m_registeredWrapperTickFuncs.Where(_wrapperTickFunc => _wrapperTickFunc.Type == s_curGamemode.GetType()))
            {
                Debug.WriteLine($"Unregistering gamemode tick function {wrapperTickFunc.MethodInfo.Name} for {wrapperTickFunc.Type.Name}");

                wrapperTickFunc.TargetFunc = null;
            }

            TriggerEvent($"gamemodes:cl_sv_{s_curGamemode.EventName}_stop");

            TeamManager.DisableTeams();

            TimerManager.StopTimer();

            EntityPool.ClearEntities();

            s_curGamemode = null;
        }

        public static void RegisterGamemode(GamemodeScript _gamemode)
        {
            s_registeredGamemodes.Add(_gamemode);
        }

        public static void StopGamemode()
        {
            s_stopGamemode = true;
        }
    }
}