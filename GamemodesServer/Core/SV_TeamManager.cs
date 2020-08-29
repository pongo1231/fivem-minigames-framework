using CitizenFX.Core;
using GamemodesShared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamemodesServer.Core
{
    public class TeamManager : GmScript
    {
        private class TeamPlayer
        {
            public TeamPlayer(Player _player, ETeamType _teamType)
            {
                Player = _player;
                TeamType = _teamType;
            }

            public Player Player { get; private set; }
            public ETeamType TeamType { get; private set; } = ETeamType.TEAM_UNK;
        }

        private static List<TeamPlayer> s_teamPlayers = new List<TeamPlayer>();
        private static bool s_enableTeams = false;
        public static bool TeamsLoaded { get; private set; } = false;

        [PlayerDropped]
        private void OnPlayerDropped(Player _player)
        {
            s_teamPlayers.RemoveAll(teamPlayer => teamPlayer.Player == _player);
        }

        [Tick]
        private async Task OnTickHandleTeams()
        {
            if (!s_enableTeams)
            {
                return;
            }

            foreach (Player player in PlayerLoadStateManager.GetLoadedInPlayers())
            {
                if (s_teamPlayers.Find(teamPlayer => teamPlayer.Player == player) == null)
                {
                    ETeamType teamType = ETeamType.TEAM_UNK;

                    int team1Count = s_teamPlayers.FindAll(teamPlayer => teamPlayer.TeamType == ETeamType.TEAM_RED).Count;
                    int team2Count = s_teamPlayers.FindAll(teamPlayer => teamPlayer.TeamType == ETeamType.TEAM_BLUE).Count;

                    if (team1Count < team2Count)
                    {
                        teamType = ETeamType.TEAM_RED;
                    }
                    else
                    {
                        teamType = ETeamType.TEAM_BLUE;
                    }

                    s_teamPlayers.Add(new TeamPlayer(player, teamType));

                    await PlayerResponseAwaiter.AwaitResponse(player, "gamemodes:cl_sv_setteam", "gamemodes:cl_sv_gotteam", (int)teamType);
                }
            }

            TeamsLoaded = true;

            await Task.FromResult(0);
        }

        [Tick]
        private async Task OnTickBroadcastTeamStates()
        {
            List<SHTeamPlayer> sharedTeamPlayers = new List<SHTeamPlayer>();

            foreach (TeamPlayer teamPlayer in s_teamPlayers)
            {
                sharedTeamPlayers.Add(new SHTeamPlayer(int.Parse(teamPlayer.Player.Handle), (int)teamPlayer.TeamType));
            }

            TriggerClientEvent("gamemodes:cl_sv_syncteams", sharedTeamPlayers);

            await Delay(500);
        }

        public static ETeamType GetPlayerTeam(Player _player)
        {
            TeamPlayer teamPlayer = s_teamPlayers.Find(_teamPlayer => _teamPlayer.Player == _player);

            return teamPlayer == null ? ETeamType.TEAM_UNK : teamPlayer.TeamType;
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
