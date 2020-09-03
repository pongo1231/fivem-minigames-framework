using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using GamemodesClient.Core;
using GamemodesClient.Core.Gamemode;
using GamemodesClient.Utils;
using GamemodesShared;
using GamemodesShared.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Font = CitizenFX.Core.UI.Font;

namespace GamemodesClient.Gamemodes
{
    /// <summary>
    /// Knockdown gamemode class
    /// </summary>
    public class Knockdown : GamemodeScript
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
        /// List of obstacle network ids sent by server
        /// </summary>
        private List<GmNetEntity<Prop>> m_obstacles = new List<GmNetEntity<Prop>>();

        /// <summary>
        /// Time until no collisions mode runs out
        /// </summary>
        private int m_noCollisionsTimestamp;

        /// <summary>
        /// Constructor
        /// </summary>
        public Knockdown() : base("knockdown", null)
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
            m_obstacles.Clear();

            // Request a scooter from server
            TriggerServerEvent("gamemodes:sv_cl_requestscooter", SpawnManager.SpawnPos, SpawnManager.SpawnRot);

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
        [EventHandler("gamemodes:cl_sv_knockdown_updatescores")]
        private void OnUpdateScores(int _blueGoals, int _redGoals)
        {
            // Set score text
            m_goalsText.Caption = $"~r~{_redGoals}   ~b~{_blueGoals}";
        }

        /// <summary>
        /// Set fall off height event by server
        /// </summary>
        /// <param name="_fallOffHeight">Fall off height</param>
        [EventHandler("gamemodes:cl_sv_knockdown_setfalloffheight")]
        private void OnSetFallOffHeight(float _fallOffHeight)
        {
            // Set fall off height
            m_fallOffHeight = _fallOffHeight;
        }

        /// <summary>
        /// Spawned obstacle event by server
        /// </summary>
        /// <param name="_networkId">Network id of obstacle</param>
        [EventHandler("gamemodes:cl_sv_knockdown_spawnedobstacle")]
        private void OnServerSpawnedObstacle(int _networkId)
        {
            GmNetEntity<Prop> obstacle = new GmNetEntity<Prop>(_networkId, true);

            obstacle.Entity.Opacity = 0;
        }

        /// <summary>
        /// Update obstacles list event by server
        /// </summary>
        /// <param name="_obstacles">List of obstacle network ids</param>
        [EventHandler("gamemodes:cl_sv_knockdown_updateobstacles")]
        private void OnSpawnedObstacles(List<dynamic> _obstacles)
        {
            // Clear obstacles
            m_obstacles.Clear();

            // Add all network ids to list
            foreach (dynamic networkId in _obstacles)
            {
                m_obstacles.Add(new GmNetEntity<Prop>(networkId, true));
            }
        }

        /// <summary>
        /// Tick function
        /// </summary>
        [GamemodeTick]
        private async Task OnTick()
        {
            // Check if not in prestart camera
            if (!IsGamemodePreStartRunning)
            {
                // Draw score text
                m_goalsText.Draw();

                // Draw mission objective text corresponding to team
                if (TeamManager.TeamType == ETeamType.TEAM_RED)
                {
                    ScreenUtils.ShowSubtitle("Don't fall off to avoid giving the ~b~Blue Team~w~ points!");
                }
                else if (TeamManager.TeamType == ETeamType.TEAM_BLUE)
                {
                    ScreenUtils.ShowSubtitle("Don't fall off to avoid giving the ~r~Red Team~w~ points!");
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
                        TriggerServerEvent("gamemodes:sv_cl_knockdown_felloff");

                        m_noCollisionsTimestamp = API.GetGameTimer() + 3000;

                        // Respawn scooter
                        await SpawnManager.Respawn();
                    }
                }

                // No camping allowed
                float scooterRoll = scooter.Entity.Rotation.Y;
                API.SetVehicleReduceGrip(scooter.Entity.Handle, scooter.Entity.Speed < 5f && (scooterRoll < -10f || scooterRoll > 10f));
            }

            /* Handle obstacles */

            foreach (GmNetEntity<Prop> obstacle in m_obstacles)
            {
                // Check if it (still) exists
                if (obstacle.Exists)
                {
                    // Disable collisions with current scooter if in no collisions mode
                    if (scooter.Exists && m_noCollisionsTimestamp > API.GetGameTimer())
                    {
                        API.SetEntityNoCollisionEntity(obstacle.Entity.Handle, scooter.Entity.Handle, true);
                    }

                    // Hide if too far away
                    float dist = MathUtils.GetDistance(obstacle.Entity.Position, Game.PlayerPed.Position);
                    if (dist > 250f)
                    {
                        obstacle.Entity.Opacity = Math.Max((int)(255 - (dist - 250f) * 25.5f), 0);
                    }
                    else if (obstacle.Entity.Opacity != 255)
                    {
                        obstacle.Entity.ResetOpacity();
                    }
                }
            }

            await Task.FromResult(0);
        }

        /// <summary>
        /// Tick function for handling no collisions mode anim
        /// </summary>
        [GamemodeTick]
        private async Task OnTickIndicateNoCollisionsMode()
        {
            // Get scooter
            GmNetEntity<Vehicle> scooter = PlayerScooterManager.CurrentScooter;

            // Play no collisions mode animation if appropriate
            if (scooter.Exists && m_noCollisionsTimestamp > API.GetGameTimer())
            {
                scooter.Entity.FadeIn();
            }

            await Delay(200);
        }
    }
}