using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesServer.Core;
using GamemodesServer.Core.Gamemode;
using GamemodesServer.Utils;
using GamemodesShared;
using System.Linq;
using System.Threading.Tasks;
using static GamemodesServer.Gamemodes.Hoops.Hoops_Map;

namespace GamemodesServer.Gamemodes.Hoops
{
    /// <summary>
    /// Hoops gamemode class
    /// </summary>
    public class Hoops : GamemodeScript<Hoops_Map>
    {
        /// <summary>
        /// Red score
        /// </summary>
        private int m_redScore;

        /// <summary>
        /// Blue score
        /// </summary>
        private int m_blueScore;

        /// <summary>
        /// Array of hoops
        /// </summary>
        private Hoop[] m_hoops;

        /// <summary>
        /// Constructor
        /// </summary>
        public Hoops()
        {
            Name = "Hoop Da Loop";
            Description = "Collect hoops whilst performing stunts!";
            EventName = "hoops";
            TimerSeconds = 120;
        }

        /// <summary>
        /// Pre start function
        /// </summary>
        [GamemodePreStart]
        private async Task OnPreStart()
        {
            // Reset scores
            m_redScore = 0;
            m_blueScore = 0;

            // Copy hoops from current map
            m_hoops = CurrentMap.Hoops;

            // Activate all hoops again
            foreach (Hoop hoop in m_hoops)
            {
                hoop.IsActive = true;
            }

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

        /// <summary>
        /// Timer up function
        /// </summary>
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

        /// <summary>
        /// Get winner team
        /// </summary>
        /// <returns>Winner team</returns>
        public override ETeamType GetWinnerTeam()
        {
            return m_redScore > m_blueScore ? ETeamType.TEAM_RED : ETeamType.TEAM_BLUE;
        }

        /// <summary>
        /// Tick function for handling hoops
        /// </summary>
        [GamemodeTick]
        private async Task OnTickHandleHoops()
        {
            // Get current time stamp
            long curTimestamp = API.GetGameTimer();

            // Iterate through each player
            foreach (Player player in PlayerLoadStateManager.GetLoadedInPlayers())
            {
                // Get player position
                Vector3 playerPos = player.Character.Position;

                // Iterate through all active hoops
                foreach (Hoop hoop in m_hoops.Where(_hoop => _hoop.IsActive))
                {
                    // Get hoop position
                    Vector3 hoopPos = hoop.Position;

                    // Check if player is inside hoop
                    if (playerPos.IsInArea(hoopPos - 3f, hoopPos + 3f))
                    {
                        // Get player team
                        ETeamType playerTeam = player.GetTeam();

                        // Add points according to player team and hoop type
                        if (playerTeam == ETeamType.TEAM_RED)
                        {
                            m_redScore += hoop.IsExtraWorth ? 5 : 1;
                        }
                        else if (playerTeam == ETeamType.TEAM_BLUE)
                        {
                            m_blueScore += hoop.IsExtraWorth ? 5 : 1;
                        }

                        // Stop gamemode if in overtime
                        if (TimerManager.InOvertime)
                        {
                            StopGamemode();
                        }

                        // Disable hoop for some time
                        hoop.IsActive = false;
                        hoop.RespawnTimestamp = hoop.IsExtraWorth ? long.MaxValue : curTimestamp + 30000;

                        // Notify player of hoop collection
                        player.TriggerEvent("gamemodes:cl_sv_hoops_collectedhoop", hoop.IsExtraWorth);

                        // Respawn all hoops (except this one) if there are none left
                        if (m_hoops.Where(_hoop => _hoop.IsActive).Count() == 0)
                        {
                            foreach (Hoop _hoop in m_hoops.Where(__hoop => __hoop != hoop && !__hoop.IsExtraWorth))
                            {
                                _hoop.IsActive = true;
                            }
                        }
                    }
                }
            }

            // Enable all disabled hoops if time is over
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
