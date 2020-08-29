using CitizenFX.Core;
using GamemodesServer.Core.Map;

namespace GamemodesServer.Gamemodes.Scooterball
{
    public abstract class Scooterball_Map : GamemodeMap
    {
        public Vector3 BallSpawnPos { get; protected set; }

        public float FallOffHeight { get; protected set; }

        public Vector3 RedGoalPos1 { get; protected set; }
        public Vector3 RedGoalPos2 { get; protected set; }

        public Vector3 BlueGoalPos1 { get; protected set; }
        public Vector3 BlueGoalPos2 { get; protected set; }
    }
}
