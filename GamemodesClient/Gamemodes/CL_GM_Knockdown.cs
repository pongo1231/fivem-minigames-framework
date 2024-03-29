﻿using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesClient.Core;
using GamemodesClient.Core.Gamemode;
using GamemodesClient.Utils;
using GamemodesShared;
using GamemodesShared.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamemodesClient.Gamemodes
{
    /// <summary>
    /// Knockdown gamemode
    /// </summary>
    public class Knockdown : GamemodeScript
    {
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
            // Clear obstacles
            m_obstacles.Clear();

            // Request a scooter from server
            TriggerServerEvent("gamemodes:sv_cl_requestscooter", SpawnManager.SpawnPos,
                SpawnManager.SpawnRot);

            await Task.FromResult(0);
        }

        /// <summary>
        /// Spawned obstacle event by server
        /// </summary>
        /// <param name="_networkId">Network id of obstacle</param>
        [EventHandler("gamemodes:cl_sv_knockdown_spawnedobstacle")]
        private void OnServerSpawnedObstacle(int _networkId)
        {
            var obstacle = new GmNetEntity<Prop>(_networkId, true);

            obstacle.Entity.Opacity = 0;
        }

        /// <summary>
        /// Update obstacles list event by server
        /// </summary>
        /// <param name="_obstacles">List of obstacle network ids</param>
        [EventHandler("gamemodes:cl_sv_knockdown_updateobstacles")]
        private void OnUpdateObstacles(List<dynamic> _obstacles)
        {
            // Clear obstacles
            m_obstacles.Clear();

            // Add all network ids to list
            foreach (var networkId in _obstacles)
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
                // Draw mission objective text corresponding to team
                if (TeamManager.TeamType == ETeamType.TEAM_RED)
                {
                    ScreenUtils.ShowSubtitle(
                        "Don't fall off to avoid giving the ~b~Blue Team~w~ points!");
                }
                else if (TeamManager.TeamType == ETeamType.TEAM_BLUE)
                {
                    ScreenUtils.ShowSubtitle(
                        "Don't fall off to avoid giving the ~r~Red Team~w~ points!");
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

                // Check if prestart camera is not running
                if (!IsGamemodePreStartRunning)
                {
                    // Check if scooter below min height or dead
                    if (PlayerScooterManager.RespawnedThisFrame)
                    {
                        TriggerServerEvent("gamemodes:sv_cl_knockdown_felloff");

                        m_noCollisionsTimestamp = API.GetGameTimer() + 3000;
                    }
                }

                // Make player's vehicle slide in case they are trying to camp somewhere
                var scooterRoll = scooter.Entity.Rotation.Y;
                API.SetVehicleReduceGrip(scooter.Entity.Handle,
                    scooter.Entity.Speed < 5f && (scooterRoll < -10f || scooterRoll > 10f));
            }

            /* Handle obstacles */

            foreach (var obstacle in m_obstacles)
            {
                // Check if it (still) exists
                if (obstacle.Exists)
                {
                    // Disable collisions with current scooter if in no collisions mode
                    if (scooter.Exists && m_noCollisionsTimestamp > API.GetGameTimer())
                    {
                        API.SetEntityNoCollisionEntity(obstacle.Entity.Handle,
                            scooter.Entity.Handle, true);
                    }

                    // Hide if too far away
                    var dist = MathUtils.GetDistance(obstacle.Entity.Position,
                        Game.PlayerPed.Position);
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
            var scooter = PlayerScooterManager.CurrentScooter;

            // Play no collisions mode animation if appropriate
            if (scooter.Exists && m_noCollisionsTimestamp > API.GetGameTimer())
            {
                scooter.Entity.FadeIn();
            }

            await Delay(200);
        }
    }
}