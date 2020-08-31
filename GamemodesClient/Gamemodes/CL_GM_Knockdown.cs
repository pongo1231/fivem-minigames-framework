using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using GamemodesClient.Core;
using GamemodesClient.Core.Gamemode;
using GamemodesClient.Utils;
using GamemodesShared;
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
        /// Scooter entity of player
        /// </summary>
        private GmNetEntity<Vehicle> m_scooter;

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
        private List<int> m_obstacles = new List<int>();

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
            m_scooter = default;
            m_fallOffHeight = float.MaxValue;

            // Clear obstacles
            m_obstacles.Clear();

            // Request a scooter from server
            TriggerServerEvent("gamemodes:sv_cl_knockdown_requestscooter", SpawnManager.SpawnPos, SpawnManager.SpawnRot);

            await Task.FromResult(0);
        }

        /// <summary>
        /// Start function
        /// </summary>
        [GamemodeStart]
        private async Task OnStart()
        {
            // Enable boost
            //BoostManager.BoostEnabled = true;

            await Task.FromResult(0);
        }

        /// <summary>
        /// Pre stop function
        /// </summary>
        [GamemodePreStop]
        private async Task OnPreStop()
        {
            // Disable boost
            //BoostManager.BoostEnabled = false;

            await Task.FromResult(0);
        }

        /// <summary>
        /// Spawn scooter event by server
        /// </summary>
        /// <param name="_netId">Network id of scooter</param>
        [EventHandler("gamemodes:cl_sv_knockdown_spawnedscooter")]
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
            //BoostManager.BoostVehicle = m_scooter;

            // Fade in screen
            await ScreenUtils.FadeIn();
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
        /// Update obstacles list event by server
        /// </summary>
        /// <param name="_obstacles">List of obstacle network ids</param>
        [EventHandler("gamemodes:cl_sv_knockdown_updateobstacles")]
        private void OnSpawnedBall(List<dynamic> _obstacles)
        {
            // Clear obstacles
            m_obstacles.Clear();

            // Add all network ids to list
            foreach (dynamic networkId in _obstacles)
            {
                m_obstacles.Add(networkId);
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

            // Check if scooter exists
            if (m_scooter.Exists)
            {
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

                    // Check if scooter below min height or dead
                    if (m_scooter.Entity.Position.Z < m_fallOffHeight || m_scooter.Entity.IsDead)
                    {
                        TriggerServerEvent("gamemodes:sv_cl_knockdown_felloff");

                        m_noCollisionsTimestamp = API.GetGameTimer() + 3000;

                        // Respawn scooter
                        await SpawnManager.Respawn();
                    }
                }
            }

            /* Handle obstacles */

            foreach (int obstacleNetworkId in m_obstacles)
            {
                // Get entity
                GmNetEntity<Prop> prop = new GmNetEntity<Prop>(obstacleNetworkId, true);

                // Check if it (still) exists
                if (prop.Exists)
                {
                    // Disable collisions with current scooter if in no collisions mode
                    if (m_scooter.Exists && m_noCollisionsTimestamp > API.GetGameTimer())
                    {
                        API.SetEntityNoCollisionEntity(prop.Entity.Handle, m_scooter.Entity.Handle, true);
                    }
                }
            }

            await Task.FromResult(0);
        }

        [GamemodeTick]
        private async Task OnTickIndicateNoCollisionsMode()
        {
            if (m_scooter.Exists && m_noCollisionsTimestamp > API.GetGameTimer())
            {
                m_scooter.Entity.FadeIn();
            }

            await Delay(200);
        }
    }
}