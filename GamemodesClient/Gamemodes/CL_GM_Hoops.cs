using CitizenFX.Core;
using CitizenFX.Core.UI;
using GamemodesClient.Core;
using GamemodesClient.Core.Gamemode;
using GamemodesClient.Utils;
using GamemodesShared;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace GamemodesClient.Gamemodes
{
    /// <summary>
    /// Hoops gamemode
    /// </summary>
    public class Hoops : GamemodeScript
    {
        /// <summary>
        /// List of hoops
        /// </summary>
        private List<dynamic> m_hoops = new List<dynamic>();

        /// <summary>
        /// List of hoop blips
        /// </summary>
        private List<Blip> m_blips = new List<Blip>();

        /// <summary>
        /// Constructor
        /// </summary>
        public Hoops() : base("hoops",
            "~r~Red Hoops~w~ are worth 1 point.\r~g~Green Hoops~w~ are worth 5 points.")
        {

        }

        /// <summary>
        /// Pre start function
        /// </summary>
        [GamemodePreStart]
        private async Task OnPreStart()
        {
            // Clear hoops
            m_hoops.Clear();

            // Request a scooter from server
            TriggerServerEvent("gamemodes:sv_cl_requestscooter", SpawnManager.SpawnPos,
                SpawnManager.SpawnRot);

            await Task.FromResult(0);
        }

        /// <summary>
        /// Pre stop function
        /// </summary>
        [GamemodePreStop]
        private async Task OnPreStop()
        {
            /* Clear all hoop blips */

            foreach (var blip in m_blips)
            {
                blip.Delete();
            }

            m_blips.Clear();

            await Task.FromResult(0);
        }

        /// <summary>
        /// Update active hoops list event by server
        /// </summary>
        /// <param name="_obstacles">List of hoop objects</param>
        [EventHandler("gamemodes:cl_sv_hoops_updatehoops")]
        private void OnUpdateObstacles(List<dynamic> _hoops)
        {
            // Update hoops list
            m_hoops = _hoops;

            /* Remove all existing hoop blips */

            foreach (var blip in m_blips)
            {
                blip.Delete();
            }

            m_blips.Clear();

            // Create new blips for all active hoops if gamemode is started
            if (!IsGamemodePreStartRunning)
            {
                foreach (var hoop in m_hoops)
                {
                    var blip = World.CreateBlip(hoop.Position);
                    blip.Scale = 0.75f;
                    blip.Color = hoop.IsExtraWorth ? BlipColor.Green : BlipColor.Red;
                    blip.Name = "Hoop";

                    m_blips.Add(blip);
                }
            }
        }

        /// <summary>
        /// Collected hoop event by server
        /// </summary>
        /// <param name="_isExtraWorth">If hoop gives extra points</param>
        [EventHandler("gamemodes:cl_sv_hoops_collectedhoop")]
        private void OnCollectHoop(bool _isExtraWorth)
        {
            // Play collection sound
            AudioUtils.PlayFrontendAudio("GTAO_Shepherd_Sounds", "Checkpoint_Teammate");

            // Show notification
            Screen.ShowNotification(_isExtraWorth ? "~g~You collected a green hoop!"
                : "You collected a hoop!");
        }

        /// <summary>
        /// Tick function
        /// </summary>
        [GamemodeTick]
        private async Task OnTick()
        {
            // Check if gamemode has started
            if (!IsGamemodePreStartRunning)
            {
                // Draw mission objective text corresponding to team
                if (TeamManager.TeamType == ETeamType.TEAM_RED)
                {
                    ScreenUtils.ShowSubtitle(
                        "Collect hoops to score points for the ~r~Red Team~w~!");
                }
                else if (TeamManager.TeamType == ETeamType.TEAM_BLUE)
                {
                    ScreenUtils.ShowSubtitle(
                        "Collect hoops to score points for the ~b~Blue Team~w~!");
                }

                // Draw all active hoops
                foreach (var hoop in m_hoops)
                {
                    World.DrawMarker(MarkerType.VerticleCircle, hoop.Position, Vector3.Zero,
                        Vector3.Zero, new Vector3(7f, 7f, 7f),
                        hoop.IsExtraWorth ? Color.FromArgb(127, 0, 255, 0)
                            : Color.FromArgb(127, 255, 0, 0), false, true);
                }
            }

            // Get scooter
            var scooter = PlayerScooterManager.CurrentScooter;

            // Check if scooter exists
            if (scooter.Exists)
            {
                // Set player into scooter if not already done so
                if (scooter.Entity.Driver != Game.PlayerPed)
                {
                    Game.PlayerPed.SetIntoVehicle(scooter.Entity, VehicleSeat.Driver);
                }
            }

            await Task.FromResult(0);
        }
    }
}
