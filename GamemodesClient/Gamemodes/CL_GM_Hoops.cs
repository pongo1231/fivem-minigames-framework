using CitizenFX.Core;
using CitizenFX.Core.UI;
using GamemodesClient.Core;
using GamemodesClient.Core.Gamemode;
using GamemodesClient.Utils;
using GamemodesShared;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Font = CitizenFX.Core.UI.Font;

namespace GamemodesClient.Gamemodes
{
    /// <summary>
    /// Hoops gamemode class
    /// </summary>
    public class Hoops : GamemodeScript
    {
        /// <summary>
        /// Score text
        /// </summary>
        private Text m_goalsText = new Text(null, new PointF(640f, 50f), 1.5f, Color.FromArgb(255, 255, 255), Font.Pricedown, Alignment.Center, true, true);

        /// <summary>
        /// Max height before player is considered as off map
        /// </summary>
        private float m_fallOffHeight = float.MaxValue;

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
        public Hoops() : base("hoops", "~r~Red Hoops~w~ are worth 1 point.\r~g~Green Hoops~w~ are worth 5 points.")
        {

        }

        /// <summary>
        /// Pre start function
        /// </summary>
        [GamemodePreStart]
        private async Task OnPreStart()
        {
            // Reset variables
            m_fallOffHeight = float.MaxValue;

            // Clear hoops
            m_hoops.Clear();

            // Request a scooter from server
            TriggerServerEvent("gamemodes:sv_cl_requestscooter", SpawnManager.SpawnPos, SpawnManager.SpawnRot);

            await Task.FromResult(0);
        }

        /// <summary>
        /// Pre stop function
        /// </summary>
        [GamemodePreStop]
        private async Task OnPreStop()
        {
            /* Clear all blips */

            foreach (Blip blip in m_blips)
            {
                blip.Delete();
            }

            m_blips.Clear();

            await Task.FromResult(0);
        }

        /// <summary>
        /// Stop function
        /// </summary>
        [GamemodeStop]
        private async Task OnStop()
        {
            // Cleanup scooter
            PlayerScooterManager.Cleanup();

            await Task.FromResult(0);
        }

        /// <summary>
        /// Update scores event by server
        /// </summary>
        /// <param name="_blueGoals">Blue score</param>
        /// <param name="_redGoals">Red score</param>
        [EventHandler("gamemodes:cl_sv_hoops_updatescores")]
        private void OnUpdateScores(int _blueGoals, int _redGoals)
        {
            // Set score text
            m_goalsText.Caption = $"~r~{_redGoals}   ~b~{_blueGoals}";
        }

        /// <summary>
        /// Set fall off height event by server
        /// </summary>
        /// <param name="_fallOffHeight">Fall off height</param>
        [EventHandler("gamemodes:cl_sv_hoops_setfalloffheight")]
        private void OnSetFallOffHeight(float _fallOffHeight)
        {
            // Set fall off height
            m_fallOffHeight = _fallOffHeight;
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

            foreach (Blip blip in m_blips)
            {
                blip.Delete();
            }

            m_blips.Clear();

            // Create new blips for all active hoops if gamemode is started
            if (!IsGamemodePreStartRunning)
            {
                foreach (dynamic hoop in m_hoops)
                {
                    Blip blip = World.CreateBlip(hoop.Position);
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
            Screen.ShowNotification(_isExtraWorth ? "~g~You collected a green hoop!" : "You collected a hoop!");
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
                // Draw score text
                m_goalsText.Draw();

                // Draw mission objective text corresponding to team
                if (TeamManager.TeamType == ETeamType.TEAM_RED)
                {
                    ScreenUtils.ShowSubtitle("Collect hoops to score points for the ~r~Red Team~w~!");
                }
                else if (TeamManager.TeamType == ETeamType.TEAM_BLUE)
                {
                    ScreenUtils.ShowSubtitle("Collect hoops to score points for the ~b~Blue Team~w~!");
                }

                // Draw all active hoops
                foreach (dynamic hoop in m_hoops)
                {
                    World.DrawMarker(MarkerType.VerticleCircle, hoop.Position, Vector3.Zero, Vector3.Zero, new Vector3(7f, 7f, 7f),
                        hoop.IsExtraWorth ? Color.FromArgb(127, 0, 255, 0) : Color.FromArgb(127, 255, 0, 0), false, true);
                }
            }

            // Disable cinematic camera
            Game.DisableControlThisFrame(1, Control.VehicleCinCam);

            // Get scooter
            GmNetEntity<Vehicle> scooter = PlayerScooterManager.CurrentScooter;

            // Check if scooter exists
            if (scooter.Exists)
            {
                // Set player into scooter if not already done so
                if (scooter.Entity.Driver != Game.PlayerPed)
                {
                    Game.PlayerPed.SetIntoVehicle(scooter.Entity, VehicleSeat.Driver);
                }

                // Check if prestart camera is not running
                if (!IsGamemodePreStartRunning)
                {
                    // Disable handbrakes
                    Game.DisableControlThisFrame(1, Control.VehicleHandbrake);

                    // Check if scooter below min height or dead
                    if (scooter.Entity.Position.Z < m_fallOffHeight || scooter.Entity.IsDead)
                    {
                        // Respawn scooter
                        await SpawnManager.Respawn();
                    }
                }
            }

            await Task.FromResult(0);
        }
    }
}
