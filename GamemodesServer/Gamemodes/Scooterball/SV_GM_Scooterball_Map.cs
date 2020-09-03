using CitizenFX.Core;
using GamemodesServer.Core.Map;
using System.Collections.Generic;

namespace GamemodesServer.Gamemodes.Scooterball
{
    /// <summary>
    /// Scooterball Map class
    /// </summary>
    public abstract class Scooterball_Map : GamemodeMap
    {
        /// <summary>
        /// Position to spawn ball at
        /// </summary>
        public Vector3 BallSpawnPos { get; protected set; }

        /// <summary>
        /// Height at which the ball we respawn
        /// </summary>
        public float FallOffHeight { get; protected set; }

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
    }
}
