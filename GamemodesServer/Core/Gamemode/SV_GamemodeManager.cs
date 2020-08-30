using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesServer.Utils;
using GamemodesShared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamemodesServer.Core.Gamemode
{
    /// <summary>
    /// Gamemode manager class
    /// </summary>
    public class GamemodeManager : GmScript
    {
        /// <summary>
        /// List of players
        /// </summary>
        private List<Player> m_gamemodePlayers = new List<Player>();

        /// <summary>
        /// List of registered gamemodes
        /// </summary>
        private static List<GamemodeBaseScript> s_registeredGamemodes = new List<GamemodeBaseScript>();

        /// <summary>
        /// Currently running gamemode
        /// </summary>
        private static GamemodeBaseScript s_curGamemode = null;

        /// <summary>
        /// Whether gamemode should be stopped
        /// </summary>
        private static bool s_stopGamemode = false;

        /// <summary>
        /// Whether clients have been made aware of gamemode start yet
        /// </summary>
        private bool m_initializedGamemodeClients = false;

        /// <summary>
        /// Random
        /// </summary>
        private Random m_random = new Random();

        /// <summary>
        /// Whether we are waiting for gamemode to stop after timer has run out
        /// </summary>
        private bool m_awaitingOvertimeGamemodeStop = false;

        /// <summary>
        /// Whether prestart countdown is running
        /// </summary>
        private bool m_prestartCountdownRunning = false;

        /// <summary>
        /// Prestart countdown counter
        /// </summary>
        private int m_prestartCountdown;

        /// <summary>
        /// Player dropped function
        /// </summary>
        /// <param name="_player">Player</param>
        [PlayerDropped]
        private void OnPlayerDropped(Player _player)
        {
            // Remove player from list
            m_gamemodePlayers.Remove(_player);
        }

        /// <summary>
        /// Tick function for handling gamemode state
        /// </summary>
        [Tick]
        public async Task OnTickGamemodeHandle()
        {
            // Check if no players are left
            if (PlayerLoadStateManager.GetLoadedInPlayers().Length == 0)
            {
                // Stop gamemode if one is running right now
                if (s_curGamemode != null)
                {
                    Log.WriteLine("Stopping gamemode as everyone left.");

                    // Stop gamemode
                    await _StopGamemode();
                }

                return;
            }

            // Check for current gamemode state
            if (s_curGamemode == null)
            {
                /* A gamemode should be started now */

                // Clear players
                m_gamemodePlayers.Clear();

                // Enable teams
                TeamManager.EnableTeams();

                // Wait for teams to load
                while (!TeamManager.TeamsLoaded)
                {
                    await Delay(0);
                }

                // Check if there's a forced gamemode set as convar
                string forcedGamemodeName = API.GetConvar("gamemodes_forced_gamemode", string.Empty);
                GamemodeBaseScript forcedGamemode = s_registeredGamemodes.Find(_gamemode => _gamemode.EventName == forcedGamemodeName);

                // Choose gamemode to start
                if (forcedGamemode != null)
                {
                    // Set gamemode to forced gamemode
                    s_curGamemode = forcedGamemode;
                }
                else
                {
                    // Notify if forced gamemode couldn't be found
                    if (forcedGamemodeName != string.Empty)
                    {
                        Log.WriteLine($"Couldn't find forced gamemode {forcedGamemodeName}!");
                    }

                    // Start random gamemode
                    s_curGamemode = s_registeredGamemodes[m_random.Next(0, s_registeredGamemodes.Count)];
                }

                // Prestart gamemode
                await s_curGamemode.PreStart();

                // Set gamemode clients as not made aware yet
                m_initializedGamemodeClients = false;

                // Start countdown
                m_prestartCountdownRunning = true;
                m_prestartCountdown = 4;
            }
            else if (s_stopGamemode)
            {
                // Stop gamemode
                await _StopGamemode();
            }
            else
            {
                // Iterate through all loaded in players
                foreach (Player player in PlayerLoadStateManager.GetLoadedInPlayers())
                {
                    // Check if player wasn't made aware of gamemode yet
                    if (!m_gamemodePlayers.Contains(player))
                    {
                        // Add player to list
                        m_gamemodePlayers.Add(player);

                        // Send prestart gamemode event
                        await PlayerResponseAwaiter.AwaitResponse(player, $"gamemodes:cl_sv_{s_curGamemode.EventName}_prestart", "gamemodes:sv_cl_prestartedgamemode");

                        // Send start gamemode event if countdown isn't running
                        if (!m_prestartCountdownRunning)
                        {
                            await PlayerResponseAwaiter.AwaitResponse(player, $"gamemodes:cl_sv_{s_curGamemode.EventName}_start", "gamemodes:sv_cl_startedgamemode");
                        }
                    }
                }

                // Check if clients haven't been made aware of this gamemode yet
                if (!m_initializedGamemodeClients)
                {
                    // Show prestart cam to all clients
                    TriggerClientEvent("gamemodes:cl_sv_showprestartcam", s_curGamemode.Name, s_curGamemode.Description);

                    // Wait a bit
                    await Delay(15000);

                    // Hide prestart cam for all clients again
                    TriggerClientEvent("gamemodes:cl_sv_hideprestartcam");

                    // Flag all clients as aware
                    m_initializedGamemodeClients = true;
                }

                // Check if timer has run out and we aren't waiting for gamemode to stop yet
                if (TimerManager.HasRunOut && !m_prestartCountdownRunning && !m_awaitingOvertimeGamemodeStop)
                {
                    // Set waiting for gamemode to stop
                    m_awaitingOvertimeGamemodeStop = true;

                    // Notify gamemode of timer being up
                    s_curGamemode.TimerUp();
                }
            }

            await Task.FromResult(0);
        }

        /// <summary>
        /// Tick function for handling countdown and gamemode start after countdown
        /// </summary>
        [Tick]
        private async Task OnTickCountdown()
        {
            // Check if countdown should be running
            if (m_prestartCountdownRunning && m_initializedGamemodeClients)
            {
                // Decrement and check if countdown has reached 0
                if (m_prestartCountdown-- == 0)
                {
                    // Start gamemode
                    await s_curGamemode.Start();

                    // Wait for all clients to be aware of gamemode start
                    await PlayerResponseAwaiter.AwaitResponse($"gamemodes:cl_sv_{s_curGamemode.EventName}_start", "gamemodes:sv_cl_startedgamemode");

                    // Set countdown as not running anymore
                    m_prestartCountdownRunning = false;

                    // Start gamemode timer
                    TimerManager.SetTimer(s_curGamemode.TimerSeconds);
                }
                else
                {
                    // Update countdown
                    TriggerClientEvent("gamemodes:cl_sv_setcountdowntimer", m_prestartCountdown);
                }

                await Delay(1000);
            }
        }

        /// <summary>
        /// Stop currently running gamemode
        /// </summary>
        private async Task _StopGamemode()
        {
            // Don't set flag for waiting for gamemode to stop anymore
            s_stopGamemode = false;
            m_awaitingOvertimeGamemodeStop = false;

            // Disable teams
            TeamManager.DisableTeams();

            // Stop the timer
            TimerManager.StopTimer();

            // Prestop gamemode
            await s_curGamemode.PreStop();

            // Wait for all clients to be made aware of this
            await PlayerResponseAwaiter.AwaitResponse($"gamemodes:cl_sv_{s_curGamemode.EventName}_prestop", "gamemodes:sv_cl_prestoppedgamemode");

            // Wait a bit
            await Delay(1000);

            // Get winner team
            ETeamType winnerTeam = s_curGamemode.GetWinnerTeam();

            // Show winner cam with winner team broadcasted
            TriggerClientEvent("gamemodes:cl_sv_showwinnercam", (int)winnerTeam);

            // Wait a bit again
            await Delay(10000);

            // Hide winner cam
            TriggerClientEvent("gamemodes:cl_sv_hidewinnercam");

            // Stop gamemode
            await s_curGamemode.Stop();

            // Wait for all clients to stop gamemode
            await PlayerResponseAwaiter.AwaitResponse($"gamemodes:cl_sv_{s_curGamemode.EventName}_stop", "gamemodes:sv_cl_stoppedgamemode");

            // Once again wait a bit
            await Delay(5000);

            // Set current gamemode to none
            s_curGamemode = null;
        }

        /// <summary>
        /// Register a gamemode (should NOT be called manually)
        /// </summary>
        /// <param name="_gamemode">Gamemode to register</param>
        public static void RegisterGamemode(GamemodeBaseScript _gamemode)
        {
            // Add gamemode to list
            s_registeredGamemodes.Add(_gamemode);
        }

        /// <summary>
        /// Stop current gamemode (should NOT be called manually)
        /// </summary>
        public static void StopGamemode()
        {
            s_stopGamemode = true;
        }
    }
}