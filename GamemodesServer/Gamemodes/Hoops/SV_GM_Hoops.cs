using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesServer.Core;
using GamemodesServer.Core.Gamemode;
using GamemodesServer.Utils;
using GamemodesShared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GamemodesServer.Gamemodes.Hoops.Hoops_Map;

namespace GamemodesServer.Gamemodes.Hoops
{
    public class Hoops : GamemodeScript<Hoops_Map>
    {
        private int m_redScore;
        private int m_blueScore;

        private Hoop[] m_hoops;

        public Hoops()
        {
            Name = "Hoop Da Loop";
            Description = "Collect hoops whilst performing stunts!";
            EventName = "hoops";
            TimerSeconds = 120;
        }

        [GamemodePreStart]
        private async Task OnPreStart()
        {
            // Reset scores
            m_redScore = 0;
            m_blueScore = 0;

            m_hoops = CurrentMap.Hoops.ToArray();

            // Enable scooters
            PlayerScooterManager.Enable("rcbandito");

            await Task.FromResult(0);
        }

        /// <summary>
        /// Pre stop function
        /// </summary>
        [GamemodePreStop]
        private async Task OnPreStop()
        {
            // Disable scooters
            PlayerScooterManager.Disable();

            await Task.FromResult(0);
        }

        [GamemodeTimerUp]
        private async Task OnTimerUp()
        {
            // Go overtime if red score equals blue score
            if (m_redScore == m_blueScore)
            {
                TimerManager.SetOvertime();
            }
            else
            {
                // Stop gamemode
                StopGamemode();
            }

            await Task.FromResult(0);
        }

        public override ETeamType GetWinnerTeam()
        {
            return m_redScore > m_blueScore ? ETeamType.TEAM_RED : ETeamType.TEAM_BLUE;
        }

        [GamemodeTick]
        private async Task OnTickHandleHoops()
        {
            long curTimestamp = API.GetGameTimer();

            foreach (Player player in PlayerLoadStateManager.GetLoadedInPlayers())
            {
                Vector3 playerPos = player.Character.Position;

                foreach (Hoop hoop in m_hoops.Where(_hoop => _hoop.IsActive))
                {
                    Vector3 hoopPos = hoop.Position;

                    if (playerPos.IsInArea(hoopPos - 3f, hoopPos + 3f))
                    {
                        ETeamType playerTeam = player.GetTeam();

                        if (playerTeam == ETeamType.TEAM_RED)
                        {
                            m_redScore += hoop.IsExtraWorth ? 5 : 1;
                        }
                        else if (playerTeam == ETeamType.TEAM_BLUE)
                        {
                            m_blueScore += hoop.IsExtraWorth ? 5 : 1;
                        }

                        if (TimerManager.InOvertime)
                        {
                            StopGamemode();
                        }

                        hoop.IsActive = false;
                        hoop.RespawnTimestamp = curTimestamp + 30000;

                        player.TriggerEvent("gamemodes:cl_sv_hoops_collectedhoop", hoop.IsExtraWorth);

                        if (m_hoops.Where(_hoop => _hoop.IsActive).Count() == 0)
                        {
                            foreach (Hoop _hoop in m_hoops.Where(__hoop => __hoop != hoop))
                            {
                                _hoop.IsActive = true;
                            }
                        }
                    }
                }
            }

            foreach (Hoop hoop in m_hoops.Where(_hoop => !_hoop.IsActive && _hoop.RespawnTimestamp < curTimestamp))
            {
                hoop.IsActive = true;
            }

            await Task.FromResult(0);
        }

        /// <summary>
        /// Tick function for sending (slow) states
        /// </summary>
        [GamemodeTick]
        private async Task OnTickSendSlowEvents()
        {
            // Send fall off height to all clients
            TriggerClientEvent("gamemodes:cl_sv_hoops_setfalloffheight", CurrentMap.FallOffHeight);

            // Send scores to all clients
            TriggerClientEvent("gamemodes:cl_sv_hoops_updatescores", m_blueScore, m_redScore);

            await Delay(500);
        }

        /// <summary>
        /// Tick function for sending (fast) states
        /// </summary>
        [GamemodeTick]
        private async Task OnTickSendFastEvents()
        {
            // Send all hoops to clients
            TriggerClientEvent("gamemodes:cl_sv_hoops_updatehoops", m_hoops.Where(_hoop => _hoop.IsActive));

            await Delay(100);
        }
    }
}
