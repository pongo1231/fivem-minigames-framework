using CitizenFX.Core;
using GamemodesShared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamemodesServer
{
    public class TeamManager : GmScript
    {
        private class TeamPlayer
        {
            public TeamPlayer(Player _player, EPlayerTeamType _teamType)
            {
                Player = _player;
                TeamType = _teamType;
            }

            public Player Player { get; private set; }
            public EPlayerTeamType TeamType { get; private set; } = EPlayerTeamType.TEAM_UNK;
        }

        private static List<TeamPlayer> s_teamPlayers = new List<TeamPlayer>();
        private static bool s_enableTeams = false;
        public static bool TeamsLoaded { get; private set; } = false;

        public TeamManager()
        {
            PlayerDropped += OnPlayerDropped;
        }

        private void OnPlayerDropped(Player _player)
        {
            s_teamPlayers.RemoveAll(teamPlayer => teamPlayer.Player == _player);
        }

        [Tick]
        private async Task OnTick()
        {
            if (!s_enableTeams)
            {
                return;
            }

            foreach (Player player in PlayerLoadStateManager.GetLoadedInPlayers())
            {
                if (s_teamPlayers.Find(teamPlayer => teamPlayer.Player == player) == null)
                {
                    EPlayerTeamType teamType = EPlayerTeamType.TEAM_UNK;

                    int team1Count = s_teamPlayers.FindAll(teamPlayer => teamPlayer.TeamType == EPlayerTeamType.TEAM_RED).Count;
                    int team2Count = s_teamPlayers.FindAll(teamPlayer => teamPlayer.TeamType == EPlayerTeamType.TEAM_BLUE).Count;

                    if (team1Count < team2Count)
                    {
                        teamType = EPlayerTeamType.TEAM_RED;
                    }
                    else
                    {
                        teamType = EPlayerTeamType.TEAM_BLUE;
                    }

                    s_teamPlayers.Add(new TeamPlayer(player, teamType));

                    await PlayerResponseAwaiter.AwaitResponse(player, "gamemodes:cl_sv_setteam", "gamemodes:cl_sv_gotteam", (int)teamType);
                }
            }

            TeamsLoaded = true;

            await Task.FromResult(0);
        }

        public static EPlayerTeamType GetPlayerTeam(Player _player)
        {
            TeamPlayer teamPlayer = s_teamPlayers.Find(_teamPlayer => _teamPlayer.Player == _player);

            return teamPlayer == null ? EPlayerTeamType.TEAM_UNK : teamPlayer.TeamType;
        }

        public static void EnableTeams()
        {
            s_teamPlayers.Clear();

            s_enableTeams = true;
            TeamsLoaded = false;
        }

        public static void DisableTeams()
        {
            s_teamPlayers.Clear();

            s_enableTeams = false;
        }
    }
}
