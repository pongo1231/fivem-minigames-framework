using CitizenFX.Core;
using GamemodesShared;

namespace GamemodesServer.Core
{
    /// <summary>
    /// Extensions to Player class for Team related functions
    /// </summary>
    public static class PlayerTeamExtensions
    {
        /// <summary>
        /// Get team of player
        /// </summary>
        /// <param name="_player">Player</param>
        /// <returns>Team of player</returns>
        public static ETeamType GetTeam(this Player _player)
        {
            return TeamManager.GetPlayerTeam(_player);
        }
    }
}
