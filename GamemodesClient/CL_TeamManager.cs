using CitizenFX.Core;
using GamemodesShared;

namespace GamemodesClient
{
    public class TeamManager : BaseScript
    {
        public static EPlayerTeamType TeamType { get; private set; }

        [EventHandler("gamemodes:cl_sv_setteam")]
        private void OnSetTeam(int _teamType)
        {
            TeamType = (EPlayerTeamType)_teamType;

            TriggerServerEvent("gamemodes:cl_sv_gotteam");
        }
    }
}
