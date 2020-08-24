using CitizenFX.Core;
using GamemodesServer.Gamemodes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamemodesServer
{
    public class GamemodeManager : GmScript
    {
        private List<Player> m_gamemodePlayers = new List<Player>();

        private static List<GamemodeScript> s_registeredGamemodes = new List<GamemodeScript>();
        private static GamemodeScript s_curGamemode = null;
        private static bool s_stopGamemode = false;

        private Random m_random = new Random();

        private bool m_awaitingGamemodeStop = false;

        public GamemodeManager()
        {
            PlayerDropped += OnPlayerDropped;

            Tick += OnGamemodeHandleTick;
        }

        private void OnPlayerDropped(Player _player)
        {
            m_gamemodePlayers.Remove(_player);
        }

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

                foreach (Func<Task> tickFunc in s_curGamemode.GmTick.GetInvocationList())
                {
                    Tick += tickFunc;
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

            foreach (Func<Task> tickFunc in s_curGamemode.GmTick.GetInvocationList())
            {
                Tick -= tickFunc;
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