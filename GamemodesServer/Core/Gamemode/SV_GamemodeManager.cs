using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesServer.Utils;
using GamemodesShared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamemodesServer.Core.Gamemode
{
    public class GamemodeManager : GmScript
    {
        private List<Player> m_gamemodePlayers = new List<Player>();

        private static List<GamemodeBaseScript> s_registeredGamemodes = new List<GamemodeBaseScript>();
        private static GamemodeBaseScript s_curGamemode = null;
        private static bool s_stopGamemode = false;

        private bool m_initializedGamemodeClients = false;

        private Random m_random = new Random();

        private bool m_awaitingGamemodeStop = false;

        private bool m_prestartCountdownRunning = false;
        private int m_prestartCountdown = 4;

        [PlayerDropped]
        private void OnPlayerDropped(Player _player)
        {
            m_gamemodePlayers.Remove(_player);
        }

        [Tick]
        public async Task OnTickGamemodeHandle()
        {
            if (PlayerLoadStateManager.GetLoadedInPlayers().Length == 0)
            {
                if (s_curGamemode != null)
                {
                    Log.WriteLine("Stopping gamemode as everyone left.");

                    await _StopGamemode();
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

                string forcedGamemodeName = API.GetConvar("gamemodes_forced_gamemode", string.Empty);
                GamemodeBaseScript forcedGamemode = s_registeredGamemodes.Find(_gamemode => _gamemode.EventName == forcedGamemodeName);

                if (forcedGamemode != null)
                {
                    s_curGamemode = forcedGamemode;
                }
                else
                {
                    if (forcedGamemodeName != string.Empty)
                    {
                        Log.WriteLine($"Couldn't find forced gamemode {forcedGamemodeName}!");
                    }

                    s_curGamemode = s_registeredGamemodes[m_random.Next(0, s_registeredGamemodes.Count)];
                }

                await s_curGamemode.PreStart();

                m_initializedGamemodeClients = false;

                m_prestartCountdownRunning = true;
                m_prestartCountdown = 4;
            }
            else if (s_stopGamemode)
            {
                await _StopGamemode();
            }
            else
            {
                foreach (Player player in PlayerLoadStateManager.GetLoadedInPlayers())
                {
                    if (!m_gamemodePlayers.Contains(player))
                    {
                        m_gamemodePlayers.Add(player);

                        await PlayerResponseAwaiter.AwaitResponse(player, $"gamemodes:cl_sv_{s_curGamemode.EventName}_prestart", "gamemodes:sv_cl_prestartedgamemode");

                        if (!m_prestartCountdownRunning)
                        {
                            await PlayerResponseAwaiter.AwaitResponse(player, $"gamemodes:cl_sv_{s_curGamemode.EventName}_start", "gamemodes:sv_cl_startedgamemode");
                        }
                    }
                }

                if (!m_initializedGamemodeClients)
                {
                    TriggerClientEvent("gamemodes:cl_sv_showprestartcam", s_curGamemode.Name, s_curGamemode.Description);

                    await Delay(15000);

                    TriggerClientEvent("gamemodes:cl_sv_hideprestartcam");

                    m_initializedGamemodeClients = true;
                }

                if (TimerManager.HasRunOut && !m_prestartCountdownRunning && !m_awaitingGamemodeStop)
                {
                    m_awaitingGamemodeStop = true;

                    s_curGamemode.TimerUp();
                }
            }

            await Task.FromResult(0);
        }

        [Tick]
        private async Task OnTickCountdown()
        {
            if (m_prestartCountdownRunning && m_initializedGamemodeClients)
            {
                if (m_prestartCountdown-- == 0)
                {
                    await s_curGamemode.Start();

                    await PlayerResponseAwaiter.AwaitResponse($"gamemodes:cl_sv_{s_curGamemode.EventName}_start", "gamemodes:sv_cl_startedgamemode");

                    m_prestartCountdownRunning = false;

                    TimerManager.SetTimer(s_curGamemode.TimerSeconds);
                }
                else
                {
                    TriggerClientEvent("gamemodes:cl_sv_setcountdowntimer", m_prestartCountdown);
                }

                await Delay(1000);
            }
        }

        private async Task _StopGamemode()
        {
            s_stopGamemode = false;
            m_awaitingGamemodeStop = false;

            TeamManager.DisableTeams();

            TimerManager.StopTimer();

            await s_curGamemode.PreStop();

            await PlayerResponseAwaiter.AwaitResponse($"gamemodes:cl_sv_{s_curGamemode.EventName}_prestop", "gamemodes:sv_cl_prestoppedgamemode");

            await Delay(1000);

            ETeamType winnerTeam = s_curGamemode.GetWinnerTeam();

            TriggerClientEvent("gamemodes:cl_sv_showwinnercam", (int)winnerTeam);

            await Delay(10000);

            TriggerClientEvent("gamemodes:cl_sv_hidewinnercam");

            await s_curGamemode.Stop();

            await PlayerResponseAwaiter.AwaitResponse($"gamemodes:cl_sv_{s_curGamemode.EventName}_stop", "gamemodes:sv_cl_stoppedgamemode");

            await Delay(5000);

            s_curGamemode = null;
        }

        public static void RegisterGamemode(GamemodeBaseScript _gamemode)
        {
            s_registeredGamemodes.Add(_gamemode);
        }

        public static void StopGamemode()
        {
            s_stopGamemode = true;
        }
    }
}