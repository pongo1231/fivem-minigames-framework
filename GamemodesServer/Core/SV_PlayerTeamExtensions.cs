using CitizenFX.Core;
using GamemodesShared;

namespace GamemodesServer.Core
{
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
