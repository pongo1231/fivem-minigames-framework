using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesServer.Core;
using GamemodesServer.Core.Map;
using GamemodesServer.Utils;
using GamemodesShared.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamemodesServer.Gamemodes.Knockdown
{
    /// <summary>
    /// Knockdown Map 1 class
    /// </summary>
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

        /// <summary>
        /// First obstacle spawn pos 1
        /// </summary>
        private readonly Vector3 m_obstacleSpawnPos1_1 = new Vector3(-1717f, -3021f, 160f);

        /// <summary>
        /// First obstacle spawn pos 2
        /// </summary>
        private readonly Vector3 m_obstacleSpawnPos1_2 = new Vector3(-1775f, -2827f, 163f);

        /// <summary>
        /// First obstacle spawn velocity
        /// </summary>
        private readonly Vector3 m_obstacleSpawnPos1_Velocity = new Vector3(32f, 0f, -40f);

        /// <summary>
        /// Second obstacle spawn pos 1
        /// </summary>
        private readonly Vector3 m_obstacleSpawnPos2_1 = new Vector3(-1331f, -2827f, 160f);

        /// <summary>
        /// Second obstacle spawn pos 2
        /// </summary>
        private readonly Vector3 m_obstacleSpawnPos2_2 = new Vector3(-1280f, -3022f, 163f);

        /// <summary>
        /// Second obstacle spawn velocity
        /// </summary>
        private readonly Vector3 m_obstacleSpawnPos2_Velocity = new Vector3(-32f, 0f, -40f);

        /// <summary>
        /// List of obstacles
        /// </summary>
        private List<Obstacle> m_obstacles = new List<Obstacle>();

        /// <summary>
        /// Constructor
        /// </summary>
        public Knockdown_Map_1()
        {
            MapFileName = "knockdown/knockdown_map_1.xml";

            FallOffHeight = 53f;
        }

        /// <summary>
        /// Load function
        /// </summary>
        [MapLoad]
        private async Task OnLoad()
        {
            // Clear obstacles
            m_obstacles.Clear();

            // Also create platform players are on on the server side for better ball sync
            await EntityPool.CreateProp("stt_prop_stunt_bblock_huge_05",
                new Vector3(-1522.16589f, -2924.78613f, 74.1650925f), default, false);
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
                var prop = await EntityPool.CreateProp(
                    "stt_prop_stunt_bowling_ball", m_obstacleSpawnPos1_1, default, true);

                // Add to list
                m_obstacles.Add(new Obstacle(prop));

                // Make clients aware
                TriggerClientEvent("gamemodes:cl_sv_knockdown_spawnedobstacle", prop.NetworkId);
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
            var curTimestamp = API.GetGameTimer();

            // Delete all the obstacles which don't exist anymore
            foreach (var obstacle in m_obstacles.ToArray())
            {
                if (!obstacle.Prop.Exists())
                {
                    m_obstacles.Remove(obstacle);
                }
            }

            // Iterate through all obstacles
            foreach (var obstacle in m_obstacles)
            {
                // Check if obstacles should be respawned
                if (obstacle.Prop.Position.Z < FallOffHeight
                    || obstacle.RespawnTimestamp < curTimestamp)
                {
                    // Spawn obstacle on either left or right side
                    if (RandomUtils.RandomInt(0, 2) == 0)
                    {
                        obstacle.Prop.Position = Utils.MathUtils
                            .GetRandomPosInArea(m_obstacleSpawnPos1_1, m_obstacleSpawnPos1_2);
                        obstacle.TargetVelocity = m_obstacleSpawnPos1_Velocity;
                    }
                    else
                    {
                        obstacle.Prop.Position = Utils.MathUtils
                            .GetRandomPosInArea(m_obstacleSpawnPos2_1, m_obstacleSpawnPos2_2);
                        obstacle.TargetVelocity = m_obstacleSpawnPos2_Velocity;
                    }

                    // Reset obstacle respawn time
                    obstacle.RespawnTimestamp = curTimestamp + 30000;
                }

                // Get obstacle velocity
                var velocity = obstacle.Prop.Velocity;

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
            var obstacles = new List<int>();
            foreach (var obstacle in m_obstacles)
            {
                if (obstacle.Prop.Exists())
                {
                    obstacles.Add(obstacle.Prop.NetworkId);
                }
            }

            TriggerClientEvent("gamemodes:cl_sv_knockdown_updateobstacles", obstacles);

            await Delay(500);
        }
    }
}