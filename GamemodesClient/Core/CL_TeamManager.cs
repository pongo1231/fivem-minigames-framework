using CitizenFX.Core;
using GamemodesShared;
using System.Collections.Generic;

namespace GamemodesClient.Core
{
    public class TeamManager : BaseScript
    {
        public static ETeamType TeamType { get; private set; }

        public static List<SHTeamPlayer> TeamPlayers { get; private set; } = new List<SHTeamPlayer>();

        [EventHandler("gamemodes:cl_sv_setteam")]
        private void OnSetTeam(int _teamType)
        {
            TeamType = (ETeamType)_teamType;

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
