using CitizenFX.Core;
using GamemodesServer.Core.Map;

namespace GamemodesServer.Gamemodes.Knockdown
{
    public class Knockdown_Map : GamemodeMap
    {
        public float FallOffHeight { get; protected set; }

        public Vector3 ObstacleSpawnPos1_1 { get; protected set; }
        public Vector3 ObstacleSpawnPos1_2 { get; protected set; }
        public Vector3 ObstacleSpawnPos1_Velocity { get; protected set; }


        public Vector3 ObstacleSpawnPos2_1 { get; protected set; }
        public Vector3 ObstacleSpawnPos2_2 { get; protected set; }
        public Vector3 ObstacleSpawnPos2_Velocity { get; protected set; }

    }
}
