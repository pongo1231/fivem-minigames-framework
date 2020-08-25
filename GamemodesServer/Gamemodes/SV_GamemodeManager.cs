using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GamemodesServer.Gamemodes
{
    public class GamemodeManager : GmScript
    {
        private class WrapperTickFunc
        {
            public WrapperTickFunc(Type _type, MethodInfo _methodInfo)
            {
                Type = _type;
                Method = _methodInfo;
            }

            public Type Type { get; private set; }
            public MethodInfo Method { get; private set; }
            public Func<Task> TickFunc;
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
            foreach (MethodInfo methodInfo in Assembly.GetExecutingAssembly().GetTypes()
                .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
                .Where(method => method.GetCustomAttribute(typeof(GamemodeTickAttribute)) != null))
            {
                WrapperTickFunc wrapperTickFunc = new WrapperTickFunc(methodInfo.DeclaringType, methodInfo);

                m_registeredWrapperTickFuncs.Add(wrapperTickFunc);
            }
        }

        [PlayerDropped]
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
                    Debug.WriteLine($"Registering gamemode tick function {wrapperTickFunc.Type.Name}.{wrapperTickFunc.Method.Name}");

                    wrapperTickFunc.TickFunc = wrapperTickFunc.Method.IsStatic
                        ? (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), wrapperTickFunc.Method)
                        : (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), s_curGamemode, wrapperTickFunc.Method);

                    Tick += wrapperTickFunc.TickFunc;
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

                        await PlayerResponseAwaiter.AwaitResponse(player, $"gamemodes:cl_sv_{s_curGamemode.EventName}_start", "gamemodes:sv_cl_startedgamemode");
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
                Debug.WriteLine($"Unregistering gamemode tick function {wrapperTickFunc.Type.Name}.{wrapperTickFunc.Method.Name}");

                Tick -= wrapperTickFunc.TickFunc;

                wrapperTickFunc.TickFunc = null;
            }

            TriggerClientEvent($"gamemodes:cl_sv_{s_curGamemode.EventName}_stop");

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