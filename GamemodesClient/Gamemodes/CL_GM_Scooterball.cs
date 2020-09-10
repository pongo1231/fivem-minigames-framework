using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using GamemodesClient.Core;
using GamemodesClient.Core.Gamemode;
using GamemodesClient.Utils;
using GamemodesShared;
using System;
using System.Drawing;
using System.Threading.Tasks;
using Font = CitizenFX.Core.UI.Font;

namespace GamemodesClient.Gamemodes
{
    /// <summary>
    /// Scooterball gamemode class
    /// </summary>
    public class Scooterball : GamemodeScript
    {
        /// <summary>
        /// Ball entity
        /// </summary>
        private GmNetEntity<Prop> m_ball;
        /// <summary>
        /// Constructor
        /// </summary>
        public Scooterball() : base("scooterball", "~INPUT_VEH_ROCKET_BOOST~  -  Boost\n~INPUT_JUMP~  -  Jump")
        {

        }

        /// <summary>
        /// Pre start function
        /// </summary>
        [GamemodePreStart]
        private async Task OnPreStart()
        {
            // Reset variables
            m_ball = default;

            // Request a scooter from server
            PlayerScooterManager.Request();

            await Task.FromResult(0);
        }

        /// <summary>
        /// Start function
        /// </summary>
        [GamemodeStart]
        private async Task OnStart()
        {
            // Enable boost
            BoostManager.BoostEnabled = true;

            await Task.FromResult(0);
        }

        /// <summary>
        /// Set ball event by server
        /// </summary>
        /// <param name="_netId">Network id of ball</param>
        [EventHandler("gamemodes:cl_sv_scooterball_setball")]
        private void OnServerSetBall(int _netId)
        {
            // Store ball entity from network id
            m_ball = new GmNetEntity<Prop>(_netId, true);
        }

        /// <summary>
        /// Goal scored event by server
        /// </summary>
        /// <param name="_teamType">Team which scored a goal</param>
        /// <param name="_scorePos">Position of ball</param>
        [EventHandler("gamemodes:cl_sv_scooterball_goalscored")]
        private void OnGoalScored(int _teamType, Vector3 _scorePos)
        {
            // Create non-networked kinetic explosion which pushes player away
            Function.Call(Hash.ADD_EXPLOSION, _scorePos.X, _scorePos.Y, _scorePos.Z, 63, 100f, true, true, 2f, true);

            // Play goal ptfx
            PtfxUtils.PlayPtfxAtPos(_scorePos, "scr_rcbarry2", "scr_clown_appears", false, 3f);

            // Show notification
            Screen.ShowNotification((ETeamType)_teamType == ETeamType.TEAM_RED ? "~r~Red~w~ scored a goal!" : "~b~Blue~w~ scored a goal!");

            // Check if ball exists
            if (m_ball.Exists)
            {
                // Set ball as invisible
                m_ball.Entity.IsVisible = false;

                // Disable ball's collisions
                m_ball.Entity.IsCollisionEnabled = false;

                // Hide attached blip if existant
                if (m_ball.Entity.AttachedBlip != null)
                {
                    m_ball.Entity.AttachedBlip.Alpha = 0;
                }
            }
        }

        /// <summary>
        /// Tick function
        /// </summary>
        [GamemodeTick]
        private async Task OnTick()
        {
            //API.SetGravityLevel(1);

            // Check if not in prestart camera
            if (!IsGamemodePreStartRunning)
            {
                // Draw mission objective text corresponding to team
                if (TeamManager.TeamType == ETeamType.TEAM_RED)
                {
                    ScreenUtils.ShowSubtitle("Shoot the ~r~Ball~w~ into the ~b~Blue Goal.");
                }
                else if (TeamManager.TeamType == ETeamType.TEAM_BLUE)
                {
                    ScreenUtils.ShowSubtitle("Shoot the ~b~Ball~w~ into the ~r~Red Goal.");
                }
            }

            // Get scooter
            GmNetEntity<Vehicle> scooter = PlayerScooterManager.CurrentScooter;

            // Check if scooter exists
            if (scooter.Exists)
            {
                //m_scooter.Entity.Gravity = 7.5f;

                // Set player into scooter if not already done so
                if (scooter.Entity.Driver != Game.PlayerPed)
                {
                    Game.PlayerPed.SetIntoVehicle(scooter.Entity, VehicleSeat.Driver);
                }

                // Check if prestart camera is not running
                if (!IsGamemodePreStartRunning)
                {
                    // Check if player has pressed jump key while on ground
                    if (Game.IsControlJustPressed(1, Control.Jump) && !scooter.Entity.IsInAir)
                    {
                        // Get current velocity
                        Vector3 vel = scooter.Entity.Velocity;

                        // Set velocity Z
                        vel.Z = 7f;

                        // Apply new velocity
                        scooter.Entity.Velocity = vel;
                    }
                }
            }

            // Check if ball exists
            if (m_ball.Exists)
            {
                // Check if ball entity is visible
                if (m_ball.Entity.IsVisible)
                {
                    // Draw marker above ball with color depending on team
                    Vector3 markerPos = m_ball.Entity.Position + new Vector3(0f, 0f, 5f);
                    Color markerColor = TeamManager.TeamType == ETeamType.TEAM_RED ? Color.FromArgb(255, 0, 0) : Color.FromArgb(0, 0, 255);
                    World.DrawMarker(MarkerType.UpsideDownCone, markerPos, default, default, new Vector3(2f, 2f, 2f), markerColor, true);
                }

                // Check if blip for ball is not created yet
                if (m_ball.Entity.AttachedBlip == null)
                {
                    // Create blip for ball with color depending on team
                    Blip blip = m_ball.Entity.AttachBlip();
                    blip.Color = TeamManager.TeamType == ETeamType.TEAM_RED ? BlipColor.Red : BlipColor.Blue;
                    blip.Name = "Ball";
                    API.ShowHeightOnBlip(blip.Handle, false);
                }
            }

            await Task.FromResult(0);
        }

        /// <summary>
        /// Reset ball event by server
        /// </summary>
        [EventHandler("gamemodes:sv_cl_scooterball_resetball")]
        private void OnResetBall()
        {
            // Check if ball exists
            if (m_ball.Exists)
            {
                // Request control
                m_ball.Entity.RequestControl();

                // Set ball as visible
                m_ball.Entity.IsVisible = true;

                // Enable collisions for ball
                m_ball.Entity.IsCollisionEnabled = true;

                // Set blip as visible again if it exists
                if (m_ball.Entity.AttachedBlip != null)
                {
                    m_ball.Entity.AttachedBlip.Alpha = 255;
                }

                // Networked fade in animation for ball
                m_ball.Entity.FadeIn();
            }
        }
    }
}