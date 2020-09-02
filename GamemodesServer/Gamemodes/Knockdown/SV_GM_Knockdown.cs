using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesServer.Core;
using GamemodesServer.Core.Gamemode;
using GamemodesServer.Utils;
using GamemodesShared;
using GamemodesShared.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamemodesServer.Gamemodes.Knockdown
{
    /// <summary>
    /// Knockdown gamemode class
    /// </summary>
    public class Knockdown : GamemodeScript<Knockdown_Map>
    {
        /// <summary>
        /// Obstacle class
        /// </summary>
        private class Obstacle
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="_prop">Prop</param>
            public Obstacle(Prop _prop)
            {
                Prop = _prop;
            }

            /// <summary>
            /// Prop
            /// </summary>
            public Prop Prop { get; private set; }

            /// <summary>
            /// Time when to respawn obstacle it didn't fall off map yet
            /// </summary>
            public long RespawnTimestamp { get; set; }

            /// <summary>
            /// Target velocity of obstacle
            /// </summary>
            public Vector3 TargetVelocity { get; set; }
        }

        /// <summary>
        /// Blue score
        /// </summary>
        private int m_blueScore;

        /// <summary>
        /// Red score
        /// </summary>
        private int m_redScore;

        /// <summary>
        /// List of obstacles
        /// </summary>
        private List<Obstacle> m_obstacles = new List<Obstacle>();

        /// <summary>
        /// Constructor
        /// </summary>
        public Knockdown()
        {
            Name = "Knockdown";
            Description = "Don't get pushed off!";
            EventName = "knockdown";
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

            // Enable scooters
            PlayerScooterManager.Enable("panto");

            // Clear obstacles
            m_obstacles.Clear();

            await Task.FromResult(0);
        }

        /// <summary>
        /// Pre stop function
        /// </summary>
        [GamemodePreStop]
        public async Task OnPreStop()
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
        public override ETeamType GetWinnerTeam()
        {
            return m_redScore > m_blueScore ? ETeamType.TEAM_RED : ETeamType.TEAM_BLUE;
        }

        /// <summary>
        /// Player fell off event
        /// </summary>
        /// <param name="_player">Player</param>
        [GamemodeEventHandler("gamemodes:sv_cl_knockdown_felloff")]
        private void OnClientFellOff([FromSource]Player _player)
        {
            // Get player team
            ETeamType playerTeam = TeamManager.GetPlayerTeam(_player);

            // Increase score of opposite team (and stop gamemode if in overtime)
            if (playerTeam == ETeamType.TEAM_RED)
            {
                m_blueScore++;

                if (TimerManager.InOvertime)
                {
                    StopGamemode();
                }
            }
            else if (playerTeam == ETeamType.TEAM_BLUE)
            {
                m_redScore++;

                if (TimerManager.InOvertime)
                {
                    StopGamemode();
                }
            }
        }

        /// <summary>
        /// Tick function spawn obstacles
        /// </summary>
        [GamemodeTick]
        private async Task OnTickSpawnObstacles()
        {
            // Abort if in prestart
            if (IsGamemodePreStartRunning)
            {
                return;
            }

            // Spawn more obstacles if there are less than limit
            if (m_obstacles.Count < 50)
            {
                // Create obstacle
                Prop prop = await EntityPool.CreateProp("stt_prop_stunt_bowling_ball", CurrentMap.ObstacleSpawnPos1_1, default, true);

                // Add to list
                m_obstacles.Add(new Obstacle(prop));

                // Notify client
                TriggerClientEvent("gamemodes:sv_cl_knockdown_spawnedball", prop.NetworkId);

                /* Repeat with other side */

                prop = await EntityPool.CreateProp("stt_prop_stunt_bowling_ball", CurrentMap.ObstacleSpawnPos2_1, default, true);

                m_obstacles.Add(new Obstacle(prop));

                TriggerClientEvent("gamemodes:sv_cl_knockdown_spawnedball", prop.NetworkId);
            }

            await Task.FromResult(0);
        }

        /// <summary>
        /// Tick function for handling obstacles
        /// </summary>
        [GamemodeTick]
        private async Task OnTickHandleObstacles()
        {
            // Get current timestamp
            long curTimestamp = API.GetGameTimer();

            // Delete all the obstacles which don't exist anymore
            foreach (Obstacle obstacle in m_obstacles.ToArray())
            {
                if (!obstacle.Prop.Exists())
                {
                    m_obstacles.Remove(obstacle);
                }
            }

            // Iterate through all obstacles
            foreach (Obstacle obstacle in m_obstacles)
            {
                // Check if obstacles should be respawned
                if (obstacle.Prop.Position.Z < CurrentMap.FallOffHeight || obstacle.RespawnTimestamp < curTimestamp)
                {
                    // Spawn obstacle on either left or right side
                    if (RandomUtils.RandomInt(0, 2) == 0)
                    {
                        obstacle.Prop.Position = GetRandomPosInArea(CurrentMap.ObstacleSpawnPos1_1, CurrentMap.ObstacleSpawnPos1_2);
                        obstacle.Prop.Velocity = CurrentMap.ObstacleSpawnPos1_Velocity;
                        obstacle.TargetVelocity = CurrentMap.ObstacleSpawnPos1_Velocity;
                    }
                    else
                    {
                        obstacle.Prop.Position = GetRandomPosInArea(CurrentMap.ObstacleSpawnPos2_1, CurrentMap.ObstacleSpawnPos2_2);
                        obstacle.Prop.Velocity = CurrentMap.ObstacleSpawnPos2_Velocity;
                        obstacle.TargetVelocity = CurrentMap.ObstacleSpawnPos2_Velocity;
                    }

                    // Reset obstacle respawn time
                    obstacle.RespawnTimestamp = curTimestamp + 30000;
                }

                // Get obstacle velocity
                Vector3 velocity = obstacle.Prop.Velocity;

                // Set corresponding velocity values
                if (obstacle.TargetVelocity.X != 0f)
                {
                    velocity.X = obstacle.TargetVelocity.X;
                }

                if (obstacle.TargetVelocity.Y != 0f)
                {
                    velocity.Y = obstacle.TargetVelocity.Y;
                }

                if (obstacle.TargetVelocity.Z != 0f)
                {
                    velocity.Z = obstacle.TargetVelocity.Z;
                }

                obstacle.Prop.Velocity = velocity;
            }

            await Task.FromResult(0);
        }

        /// <summary>
        /// Tick function for sending states
        /// </summary>
        [GamemodeTick]
        private async Task OnTickSendEvents()
        {
            // Send fall off height to all clients
            TriggerClientEvent("gamemodes:cl_sv_knockdown_setfalloffheight", CurrentMap.FallOffHeight);

            // Send scores to all clients
            TriggerClientEvent("gamemodes:cl_sv_knockdown_updatescores", m_blueScore, m_redScore);

            // Send list of obstacles to all clients
            List<int> obstacles = new List<int>();
            foreach (Obstacle obstacle in m_obstacles)
            {
                if (obstacle.Prop.Exists())
                {
                    obstacles.Add(obstacle.Prop.NetworkId);
                }
            }

            TriggerClientEvent("gamemodes:cl_sv_knockdown_updateobstacles", obstacles);

            await Delay(500);
        }

        /// <summary>
        /// Get random position between 2 positions
        /// </summary>
        /// <param name="_pos1">Position 1</param>
        /// <param name="_pos2">Position 2</param>
        /// <returns></returns>
        private Vector3 GetRandomPosInArea(Vector3 _pos1, Vector3 _pos2)
        {
            // Get min and max X coords
            float minX = Math.Min(_pos1.X, _pos2.X);
            float maxX = Math.Max(_pos1.X, _pos2.X);

            // Get min and max Y coords
            float minY = Math.Min(_pos1.Y, _pos2.Y);
            float maxY = Math.Max(_pos1.Y, _pos2.Y);

            // Get min and max Z coords
            float minZ = Math.Min(_pos1.Z, _pos2.Z);
            float maxZ = Math.Max(_pos1.Z, _pos2.Z);

            // Generate random coords
            float x = RandomUtils.RandomFloat(minX, maxX);
            float y = RandomUtils.RandomFloat(minY, maxY);
            float z = RandomUtils.RandomFloat(minZ, maxZ);

            // Return vector
            return new Vector3(x, y, z);
        }
    }
}