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
    /// Knockdown Map 2 class
    /// </summary>
    public class Knockdown_Map_2 : Knockdown_Map
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
        /// Place to put obstacles first when they are spawned
        /// </summary>
        private readonly Vector3 m_obstaclePreSpawnPos = new Vector3(529f, 3888f, 160f);

        /// <summary>
        /// First obstacle spawn pos 1
        /// </summary>
        private readonly Vector3 m_obstacleSpawnPos1_1 = new Vector3(250f, 3982f, 184f);

        /// <summary>
        /// First obstacle spawn pos 2
        /// </summary>
        private readonly Vector3 m_obstacleSpawnPos1_2 = new Vector3(250f, 3847f, 184f);

        /// <summary>
        /// Second obstacle spawn pos 1
        /// </summary>
        private readonly Vector3 m_obstacleSpawnPos2_1 = new Vector3(250f, 3982f, 214f);

        /// <summary>
        /// Second obstacle spawn pos 2
        /// </summary>
        private readonly Vector3 m_obstacleSpawnPos2_2 = new Vector3(250f, 3847f, 214f);

        /// <summary>
        /// Second obstacle spawn velocity
        /// </summary>
        private readonly Vector3 m_obstacleSpawnPos_Forward = new Vector3(300f, 0f, 0f);

        /// <summary>
        /// Distance after which obstacles get despawned
        /// </summary>
        private readonly float m_obstacleDespawnDist = 500f;

        /// <summary>
        /// List of obstacles
        /// </summary>
        private List<Obstacle> m_obstacles = new List<Obstacle>();

        /// <summary>
        /// Constructor
        /// </summary>
        public Knockdown_Map_2()
        {
            MapFileName = "knockdown/knockdown_map_2.xml";

            FallOffHeight = 150f;
        }

        /// <summary>
        /// Load function
        /// </summary>
        [MapLoad]
        private async Task OnLoad()
        {
            // Clear obstacles
            m_obstacles.Clear();

            await Task.FromResult(0);
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
                    "stt_prop_stunt_bowling_ball", m_obstaclePreSpawnPos, default, true);

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
                if ((obstacle.Prop.Position - m_obstacleSpawnPos1_1).Length()
                    > m_obstacleDespawnDist || obstacle.RespawnTimestamp < curTimestamp)
                {
                    // Spawn obstacle on either left or right side
                    if (RandomUtils.RandomInt(0, 2) == 0)
                    {
                        obstacle.Prop.Position = Utils.MathUtils
                            .GetRandomPosInArea(m_obstacleSpawnPos1_1, m_obstacleSpawnPos1_2);
                    }
                    else
                    {
                        obstacle.Prop.Position = Utils.MathUtils
                            .GetRandomPosInArea(m_obstacleSpawnPos2_1, m_obstacleSpawnPos2_2);
                    }

                    obstacle.TargetVelocity = m_obstacleSpawnPos_Forward;

                    // Reset obstacle respawn time
                    obstacle.RespawnTimestamp = curTimestamp + 30000;
                }

                // Set velocity to target velocity
                obstacle.Prop.Velocity = obstacle.TargetVelocity;
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