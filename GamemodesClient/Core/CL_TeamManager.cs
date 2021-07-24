using CitizenFX.Core;
using GamemodesShared;
using System.Collections.Generic;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Manager of every player's team
    /// </summary>
    public class TeamManager : GmScript
    {
        /// <summary>
        /// Team of this player
        /// </summary>
        public static ETeamType TeamType { get; private set; }

        /// <summary>
        /// List of all team players
        /// </summary>
        public static List<SHTeamPlayer> TeamPlayers { get; private set; } = new List<SHTeamPlayer>();

        /// <summary>
        /// Set team event by server
        /// </summary>
        /// <param name="_teamType">Team to set this player to</param>
        [EventHandler("gamemodes:cl_sv_setteam")]
        private void OnSetTeam(int _teamType)
        {
            // Set team
            TeamType = (ETeamType)_teamType;

            // Respond to server
            TriggerServerEvent("gamemodes:cl_sv_gotteam");
        }

        /// <summary>
        /// Get team player list event by server
        /// </summary>
        /// <param name="_sharedTeamPlayers">List of team players</param>
        [EventHandler("gamemodes:cl_sv_syncteams")]
        private void OnSyncTeams(dynamic _sharedTeamPlayers)
        {
            // Clear current list of team players
            TeamPlayers.Clear();

            // Add a SHTeamPlayer for each team player
            foreach (var totallyNotTeamPlayer in _sharedTeamPlayers)
            {
                TeamPlayers.Add(new SHTeamPlayer(totallyNotTeamPlayer.PlayerNetId,
                    totallyNotTeamPlayer.PlayerTeamType));
            }
        }
    }
}
