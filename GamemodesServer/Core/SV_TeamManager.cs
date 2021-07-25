using CitizenFX.Core;
using GamemodesShared;
using GamemodesShared.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamemodesServer.Core
{
    /// <summary>
    /// Manager of player teams
    /// </summary>
    public class TeamManager : GmScript
    {
        /// <summary>
        /// Team player class
        /// </summary>
        private class TeamPlayer
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="_player">Player</param>
            /// <param name="_teamType">Team</param>
            public TeamPlayer(Player _player, ETeamType _teamType)
            {
                Player = _player;
                TeamType = _teamType;
            }

            /// <summary>
            /// Player
            /// </summary>
            public Player Player { get; private set; }

            /// <summary>
            /// Team of player
            /// </summary>
            public ETeamType TeamType { get; private set; } = ETeamType.TEAM_UNK;
        }

        /// <summary>
        /// List of team players
        /// </summary>
        private static List<TeamPlayer> s_teamPlayers = new List<TeamPlayer>();

        /// <summary>
        /// Whether teams are enabled
        /// </summary>
        private static bool s_enableTeams = false;

        /// <summary>
        /// Whether teams are loaded
        /// </summary>
        public static bool TeamsLoaded { get; private set; } = false;

        /// <summary>
        /// Player dropped function
        /// </summary>
        /// <param name="_player">Player</param>
        /// <param name="_dropReason">Reason for drop</param>
        [PlayerDropped]
        private void OnPlayerDropped(Player _player, string _dropReason)
        {
            // Remove player from list
            s_teamPlayers.RemoveAll(teamPlayer => teamPlayer.Player == _player);
        }

        /// <summary>
        /// Tick function for setting teams of new players
        /// </summary>
        [Tick]
        private async Task OnTickHandleTeams()
        {
            // Abort if teams aren't enabled
            if (!s_enableTeams)
            {
                return;
            }

            // Create a new list to store new players into and sort into teams randomized later
            var m_toSortIntoTeams = new List<Player>();

            // Iterate through all loaded in players
            foreach (var player in PlayerEnrollStateManager.GetLoadedInPlayers())
            {
                // Check whether this player doesn't have a team assigned to them yet
                if (s_teamPlayers.Find(teamPlayer => teamPlayer.Player == player) == null)
                {
                    // Add to list of players to sort into teams
                    m_toSortIntoTeams.Add(player);
                }
            }

            // Go through all players in to sort list in randomized order
            bool syncTeams = false;
            while (m_toSortIntoTeams.Count > 0)
            {
                // Choose random player from list
                var player = m_toSortIntoTeams[RandomUtils
                    .RandomInt(0, m_toSortIntoTeams.Count)];

                // Store type of team
                var teamType = ETeamType.TEAM_UNK;

                // Get players count of both teams
                var redCount = s_teamPlayers
                    .FindAll(teamPlayer => teamPlayer.TeamType == ETeamType.TEAM_RED).Count;
                var blueCount = s_teamPlayers
                    .FindAll(teamPlayer => teamPlayer.TeamType == ETeamType.TEAM_BLUE).Count;

                // Set player into team with fewer players
                if (redCount < blueCount)
                {
                    teamType = ETeamType.TEAM_RED;
                }
                else
                {
                    teamType = ETeamType.TEAM_BLUE;
                }

                // Add player to list
                s_teamPlayers.Add(new TeamPlayer(player, teamType));

                // Remove player from list
                m_toSortIntoTeams.Remove(player);

                // Wait for client to be aware of new team
                await PlayerResponseAwaiter.AwaitResponse(player,
                    "gamemodes:cl_sv_setteam", "gamemodes:cl_sv_gotteam", (int)teamType);

                syncTeams = true;
            }

            if (syncTeams)
            {
                // Create list
                var sharedTeamPlayers = new List<SHTeamPlayer>();

                // Store all team players in list
                foreach (var teamPlayer in s_teamPlayers)
                {
                    sharedTeamPlayers.Add(new SHTeamPlayer(int.Parse(teamPlayer.Player.Handle),
                        (int)teamPlayer.TeamType));
                }

                // Broadcast team state to all clients
                await PlayerResponseAwaiter.AwaitResponse("gamemodes:cl_sv_syncteams",
                    "gamemodes:sv_cl_gotteamsync", sharedTeamPlayers);
            }

            // Set teams as loaded
            TeamsLoaded = true;

            await Task.FromResult(0);
        }

        /// <summary>
        /// Get team of player
        /// </summary>
        /// <param name="_player">Player</param>
        /// <returns>Team of player</returns>
        public static ETeamType GetPlayerTeam(Player _player)
        {
            var teamPlayer = s_teamPlayers.Find(_teamPlayer => _teamPlayer.Player == _player);

            return teamPlayer == null ? ETeamType.TEAM_UNK : teamPlayer.TeamType;
        }

        /// <summary>
        /// Get enemy team of given team
        /// </summary>
        /// <param name="_teamType">Either red or blue team</param>
        /// <returns>Enemy team of given team or TEAM_UNK if invalid team</returns>
        public static ETeamType GetEnemyTeam(ETeamType _teamType)
        {
            switch (_teamType)
            {
                case ETeamType.TEAM_RED:
                    return ETeamType.TEAM_BLUE;
                case ETeamType.TEAM_BLUE:
                    return ETeamType.TEAM_RED;
            }

            return ETeamType.TEAM_UNK;
        }

        /// <summary>
        /// Enable teams
        /// </summary>
        public static void EnableTeams()
        {
            // Clear team players list
            s_teamPlayers.Clear();

            // Enable teams
            s_enableTeams = true;

            // Set teams as not loaded yet
            TeamsLoaded = false;
        }

        /// <summary>
        /// Disable teams
        /// </summary>
        public static void DisableTeams()
        {
            // Clear team players list
            s_teamPlayers.Clear();

            // Disable teams
            s_enableTeams = false;
        }
    }
}
