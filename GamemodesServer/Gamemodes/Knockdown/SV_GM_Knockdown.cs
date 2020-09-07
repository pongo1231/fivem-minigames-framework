using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesServer.Core;
using GamemodesServer.Core.Gamemode;
using GamemodesServer.Utils;
using GamemodesShared;
using GamemodesShared.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamemodesServer.Gamemodes.Knockdown
{
    /// <summary>
    /// Knockdown gamemode class
    /// </summary>
    public class Knockdown : GamemodeScript<Knockdown_Map>
    {
        /// <summary>
        /// Blue score
        /// </summary>
        private int m_blueScore;

        /// <summary>
        /// Red score
        /// </summary>
        private int m_redScore;

        /// <summary>
        /// Constructor
        /// </summary>
        public Knockdown()
        {
            Name = "Knockdown";
            Description = "Don't get pushed off!";
            EventName = "knockdown";
            TimerSeconds = 120;
        }

        /// <summary>
        /// Pre start function
        /// </summary>
        [GamemodePreStart]
        private async Task OnPreStart()
        {
            // Reset scores
            m_redScore = 0;
            m_blueScore = 0;

            // Enable scooters
            PlayerScooterManager.Enable("panto", CurrentMap.FallOffHeight);

            await Task.FromResult(0);
        }

        /// <summary>
        /// Pre stop function
        /// </summary>
        [GamemodePreStop]
        public async Task OnPreStop()
        {
            // Disable scooters
            PlayerScooterManager.Disable();

            await Task.FromResult(0);
        }

        /// <summary>
        /// Timer up function
        /// </summary>
        [GamemodeTimerUp]
        private async Task OnTimerUp()
        {
            // Go overtime if red score equals blue score
            if (m_redScore == m_blueScore)
            {
                TimerManager.SetOvertime();
            }
            else
            {
                // Stop gamemode
                StopGamemode();
            }

            await Task.FromResult(0);
        }

        /// <summary>
        /// Get winner team
        /// </summary>
        public override ETeamType GetWinnerTeam()
        {
            return m_redScore > m_blueScore ? ETeamType.TEAM_RED : ETeamType.TEAM_BLUE;
        }

        /// <summary>
        /// Player fell off event
        /// </summary>
        /// <param name="_player">Player</param>
        [GamemodeEventHandler("gamemodes:sv_cl_knockdown_felloff")]
        private void OnClientFellOff([FromSource]Player _player)
        {
            // Get player team
            ETeamType playerTeam = _player.GetTeam();

            // Increase score of opposite team (and stop gamemode if in overtime)
            if (playerTeam == ETeamType.TEAM_RED)
            {
                m_blueScore++;

                if (TimerManager.InOvertime)
                {
                    StopGamemode();
                }
            }
            else if (playerTeam == ETeamType.TEAM_BLUE)
            {
                m_redScore++;

                if (TimerManager.InOvertime)
                {
                    StopGamemode();
                }
            }
        }

        /// <summary>
        /// Tick function for sending states
        /// </summary>
        [GamemodeTick]
        private async Task OnTickSendEvents()
        {
            // Send scores to all clients
            TriggerClientEvent("gamemodes:cl_sv_knockdown_updatescores", m_blueScore, m_redScore);

            await Delay(500);
        }
    }
}