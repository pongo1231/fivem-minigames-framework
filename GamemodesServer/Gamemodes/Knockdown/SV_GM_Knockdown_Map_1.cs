using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesServer.Core;
using GamemodesServer.Core.Map;
using GamemodesServer.Utils;
using GamemodesShared.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamemodesServer.Gamemodes.Knockdown
{
    public class Knockdown_Map_1 : Knockdown_Map
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

        private readonly Vector3 m_obstacleSpawnPos1_1 = new Vector3(-1717f, -3021f, 160f);
        private readonly Vector3 m_obstacleSpawnPos1_2 = new Vector3(-1775f, -2827f, 163f);
        private readonly Vector3 m_obstacleSpawnPos1_Velocity = new Vector3(23f, 0f, -25f);

        private readonly Vector3 m_obstacleSpawnPos2_1 = new Vector3(-1331f, -2827f, 160f);
        private readonly Vector3 m_obstacleSpawnPos2_2 = new Vector3(-1280f, -3022f, 163f);
        private readonly Vector3 m_obstacleSpawnPos2_Velocity = new Vector3(-23f, 0f, -25f);

        /// <summary>
        /// List of obstacles
        /// </summary>
        private List<Obstacle> m_obstacles = new List<Obstacle>();

        public Knockdown_Map_1()
        {
            MapFileName = "knockdown/knockdown_map_1.xml";

            FallOffHeight = 53f;

            // Clear obstacles
            m_obstacles.Clear();
        }

        /// <summary>
        /// Load function
        /// </summary>
        [MapLoad]
        private async Task OnLoad()
        {
            // Also create platform players are on on the server side for better ball sync
            await EntityPool.CreateProp("stt_prop_stunt_bblock_huge_05", new Vector3(-1522.16589f, -2924.78613f, 74.1650925f), default, false);
        }

        /// <summary>
        /// Tick function spawn obstacles
        /// </summary>
        [MapTick]
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
                Prop prop = await EntityPool.CreateProp("stt_prop_stunt_bowling_ball", m_obstacleSpawnPos1_1, default, true);

                // Add to list
                m_obstacles.Add(new Obstacle(prop));

                // Notify client
                TriggerClientEvent("gamemodes:sv_cl_knockdown_spawnedball", prop.NetworkId);

                /* Repeat with other side */

                prop = await EntityPool.CreateProp("stt_prop_stunt_bowling_ball", m_obstacleSpawnPos2_1, default, true);

                m_obstacles.Add(new Obstacle(prop));

                TriggerClientEvent("gamemodes:sv_cl_knockdown_spawnedball", prop.NetworkId);
            }

            await Task.FromResult(0);
        }

        /// <summary>
        /// Tick function for handling obstacles
        /// </summary>
        [MapTick]
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
                if (obstacle.Prop.Position.Z < FallOffHeight || obstacle.RespawnTimestamp < curTimestamp)
                {
                    // Spawn obstacle on either left or right side
                    if (RandomUtils.RandomInt(0, 2) == 0)
                    {
                        obstacle.Prop.Position = GetRandomPosInArea(m_obstacleSpawnPos1_1, m_obstacleSpawnPos1_2);
                        obstacle.Prop.Velocity = m_obstacleSpawnPos1_Velocity;
                        obstacle.TargetVelocity = m_obstacleSpawnPos1_Velocity;
                    }
                    else
                    {
                        obstacle.Prop.Position = GetRandomPosInArea(m_obstacleSpawnPos2_1, m_obstacleSpawnPos2_2);
                        obstacle.Prop.Velocity = m_obstacleSpawnPos2_Velocity;
                        obstacle.TargetVelocity = m_obstacleSpawnPos2_Velocity;
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
        [MapTick]
        private async Task OnTickSendEvents()
        {
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
