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
        /// Scooter entity of player
        /// </summary>
        private GmNetEntity<Vehicle> m_scooter;

        /// <summary>
        /// Ball entity
        /// </summary>
        private GmNetEntity<Prop> m_ball;

        /// <summary>
        /// Score text
        /// </summary>
        private Text m_goalsText = new Text(null, new PointF(640f, 50f), 1.5f, Color.FromArgb(255, 255, 255), Font.Pricedown, Alignment.Center, true, true);

        /// <summary>
        /// Max height before player is considered as off map
        /// </summary>
        private float m_fallOffHeight = float.MaxValue;

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
            m_scooter = default;
            m_ball = default;
            m_fallOffHeight = float.MaxValue;

            // Request a scooter from server
            TriggerServerEvent("gamemodes:sv_cl_scooterball_requestscooter", SpawnManager.SpawnPos, SpawnManager.SpawnRot);

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
        /// Pre stop function
        /// </summary>
        [GamemodePreStop]
        private async Task OnPreStop()
        {
            // Disable boost
            BoostManager.BoostEnabled = false;

            await Task.FromResult(0);
        }

        /// <summary>
        /// Spawn scooter event by server
        /// </summary>
        /// <param name="_netId">Network id of scooter</param>
        [EventHandler("gamemodes:cl_sv_scooterball_spawnedscooter")]
        private async void OnServerSpawnScooter(int _netId)
        {
            // Get scooter entity from network id
            m_scooter = new GmNetEntity<Vehicle>(_netId, true);

            // Wait for scooter to exist
            while (!m_scooter.Exists)
            {
                await Delay(0);
            }

            // Request control of scooter
            m_scooter.Entity.RequestControl();

            // Set some attributes for scooter
            m_scooter.Entity.EngineHealth = float.MaxValue;
            m_scooter.Entity.LockStatus = VehicleLockStatus.StickPlayerInside;
            m_scooter.Entity.EnginePowerMultiplier = 5f;
            m_scooter.Entity.EngineTorqueMultiplier = 5f;
            m_scooter.Entity.IsEngineRunning = true;

            // Set corresponding scooter color depending on team
            if (TeamManager.TeamType == ETeamType.TEAM_RED)
            {
                m_scooter.Entity.Mods.CustomPrimaryColor = Color.FromArgb(255, 0, 0);
                m_scooter.Entity.Mods.CustomSecondaryColor = Color.FromArgb(255, 0, 0);
            }
            else if (TeamManager.TeamType == ETeamType.TEAM_BLUE)
            {
                m_scooter.Entity.Mods.CustomPrimaryColor = Color.FromArgb(0, 0, 255);
                m_scooter.Entity.Mods.CustomSecondaryColor = Color.FromArgb(0, 0, 255);
            }

            // Networked fade in animation for scooter
            m_scooter.Entity.FadeIn();

            // Set scooter as boost vehicle
            BoostManager.BoostVehicle = m_scooter;

            // Fade in screen
            await ScreenUtils.FadeIn();
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
        /// Update scores event by server
        /// </summary>
        /// <param name="_blueGoals">Blue score</param>
        /// <param name="_redGoals">Red score</param>
        [EventHandler("gamemodes:cl_sv_scooterball_updatescores")]
        private void OnUpdateScores(int _blueGoals, int _redGoals)
        {
            // Set score text
            m_goalsText.Caption = $"~r~{_redGoals}   ~b~{_blueGoals}";
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
        /// Set fall off height event by server
        /// </summary>
        /// <param name="_fallOffHeight">Fall off height</param>
        [EventHandler("gamemodes:cl_sv_scooterball_setfalloffheight")]
        private void OnSetFallOffHeight(float _fallOffHeight)
        {
            // Set fall off height
            m_fallOffHeight = _fallOffHeight;
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
                // Draw score text
                m_goalsText.Draw();

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

            // Disable cinematic camera
            Game.DisableControlThisFrame(1, Control.VehicleCinCam);

            // Check if scooter exists
            if (m_scooter.Exists)
            {
                //m_scooter.Entity.Gravity = 7.5f;

                // Set player into scooter if not already done so
                if (m_scooter.Entity.Driver != Game.PlayerPed)
                {
                    Game.PlayerPed.SetIntoVehicle(m_scooter.Entity, VehicleSeat.Driver);
                }

                // Check if prestart camera is not running
                if (!IsGamemodePreStartRunning)
                {
                    // Disable handbrakes
                    Game.DisableControlThisFrame(1, Control.VehicleHandbrake);

                    // Check if player has pressed jump key while on ground
                    if (Game.IsControlJustPressed(1, Control.Jump) && !m_scooter.Entity.IsInAir)
                    {
                        // Get current velocity
                        Vector3 vel = m_scooter.Entity.Velocity;

                        // Set velocity Z
                        vel.Z = 7f;

                        // Apply new velocity
                        m_scooter.Entity.Velocity = vel;
                    }

                    // Check if scooter below min height or dead
                    if (m_scooter.Entity.Position.Z < m_fallOffHeight || m_scooter.Entity.IsDead)
                    {
                        // Respawn scooter
                        await SpawnManager.Respawn();
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