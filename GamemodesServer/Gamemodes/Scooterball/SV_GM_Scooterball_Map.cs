using CitizenFX.Core;
using GamemodesServer.Core.Map;

namespace GamemodesServer.Gamemodes.Scooterball
{
    /// <summary>
    /// Scooterball map base class
    /// </summary>
    public abstract class Scooterball_Map : GamemodeMap
    {
        /// <summary>
        /// Position to spawn ball at
        /// </summary>
        public Vector3 BallSpawnPos { get; protected set; }

        /// <summary>
        /// Position 1 of red goal
        /// </summary>
        public Vector3 RedGoalPos1 { get; protected set; }

        /// <summary>
        /// Position 2 of red goal
        /// </summary>
        public Vector3 RedGoalPos2 { get; protected set; }

        /// <summary>
        /// Position 1 of blue goal
        /// </summary>
        public Vector3 BlueGoalPos1 { get; protected set; }

        /// <summary>
        /// Position 2 of blue goal
        /// </summary>
        public Vector3 BlueGoalPos2 { get; protected set; }

        /// <summary>
        /// The model to use for player scooters
        /// </summary>
        public string ScooterModel { get; protected set; } = "rcbandito";

        /// <summary>
        /// Map Boundary Position 1
        /// </summary>
        public Vector3 Boundary1 { get; protected set; }

        /// <summary>
        /// Map Boundary Position 2
        /// </summary>
        public Vector3 Boundary2 { get; protected set; }
    }
}
