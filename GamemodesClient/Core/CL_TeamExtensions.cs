using CitizenFX.Core;
using GamemodesShared;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Extensions to player class for Team-related things
    /// </summary>
    public static class TeamExtensions
    {
        /// <summary>
        /// Get team of player
        /// </summary>
        /// <param name="_player">Player</param>
        /// <returns>Either player's team or TEAM_UNK if not found</returns>
        public static ETeamType GetTeam(this Player _player)
        {
            // Get team player from player network id
            var teamPlayer = TeamManager.TeamPlayers
                .Find(_teamPlayer => _teamPlayer.PlayerNetId == _player.ServerId);

            // Return either player's team or unknown team if not found
            return teamPlayer != null ? (ETeamType)teamPlayer.PlayerTeamType : ETeamType.TEAM_UNK;
        }
    }
}
