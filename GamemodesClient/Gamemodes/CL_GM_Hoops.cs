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

        private List<dynamic> m_hoops = new List<dynamic>();

        private List<Blip> m_blips = new List<Blip>();

        /// <summary>
        /// Constructor
        /// </summary>
        public Hoops() : base("hoops", "~r~Red Hoops~w~ are worth 1 point.\r~g~Green Hoops~w~ are worth 2 points.")
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

            // Clear obstacles
            m_hoops.Clear();

            // Request a scooter from server
            TriggerServerEvent("gamemodes:sv_cl_requestscooter", SpawnManager.SpawnPos, SpawnManager.SpawnRot);

            await Task.FromResult(0);
        }

        /// <summary>
        /// Prestop function
        /// </summary>
        [GamemodePreStop]
        private async Task OnPreStop()
        {
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
        /// Update obstacles list event by server
        /// </summary>
        /// <param name="_obstacles">List of obstacle network ids</param>
        [EventHandler("gamemodes:cl_sv_hoops_updatehoops")]
        private void OnUpdateObstacles(List<dynamic> _hoops)
        {
            m_hoops = _hoops;

            foreach (Blip blip in m_blips)
            {
                blip.Delete();
            }

            m_blips.Clear();

            foreach (dynamic hoop in m_hoops)
            {
                Blip blip = World.CreateBlip(hoop.Position);
                blip.Scale = 0.75f;
                blip.Color = hoop.IsExtraWorth ? BlipColor.Green : BlipColor.Red;
                blip.Name = "Hoop";

                m_blips.Add(blip);
            }
        }

        [GamemodeTick]
        private async Task OnTickDrawHoops()
        {
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

                foreach (dynamic hoop in m_hoops)
                {
                    World.DrawMarker(MarkerType.VerticleCircle, hoop.Position, Vector3.Zero, hoop.Rotation, new Vector3(7f, 7f, 7f),
                        hoop.IsExtraWorth ? Color.FromArgb(0, 255, 0) : Color.FromArgb(255, 0, 0));
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
            }

            await Task.FromResult(0);
        }
    }
}
