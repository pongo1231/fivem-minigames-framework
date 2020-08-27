using CitizenFX.Core;
using GamemodesShared;
using System.Collections.Generic;

namespace GamemodesClient
{
    public class TeamManager : BaseScript
    {
        public static EPlayerTeamType TeamType { get; private set; }

        public static List<SHTeamPlayer> TeamPlayers { get; private set; } = new List<SHTeamPlayer>();

        [EventHandler("gamemodes:cl_sv_setteam")]
        private void OnSetTeam(int _teamType)
        {
            TeamType = (EPlayerTeamType)_teamType;

            TriggerServerEvent("gamemodes:cl_sv_gotteam");
        }

        [EventHandler("gamemodes:cl_sv_syncteams")]
        private void OnSyncTeams(dynamic _sharedTeamPlayers)
        {
            TeamPlayers.Clear();

            foreach (dynamic totallyNotTeamPlayer in _sharedTeamPlayers)
            {
                TeamPlayers.Add(new SHTeamPlayer(totallyNotTeamPlayer.PlayerNetId, totallyNotTeamPlayer.PlayerTeamType));
            }
        }
    }
}
