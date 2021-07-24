using CitizenFX.Core;
using GamemodesShared.Utils;
using System;

namespace GamemodesServer.Utils
{
    /// <summary>
    /// Utils for math stuff
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// Check whether target position is within position 1 and position 2
        /// </summary>
        /// <param name="_target">Target position</param>
        /// <param name="_pos1">Position 1</param>
        /// <param name="_pos2">Position 2</param>
        /// <returns>Whether target position is within the 2 positions</returns>
        public static bool IsInArea(this Vector3 _target, Vector3 _pos1, Vector3 _pos2)
        {
            return ((_target.X > _pos1.X && _target.X < _pos2.X)
                    || (_target.X < _pos1.X && _target.X > _pos2.X))
                && ((_target.Y > _pos1.Y && _target.Y < _pos2.Y)
                    || (_target.Y < _pos1.Y && _target.Y > _pos2.Y))
                && ((_target.Z > _pos1.Z && _target.Z < _pos2.Z)
                    || (_target.Z < _pos1.Z && _target.Z > _pos2.Z));
        }

        /// <summary>
        /// Get random position between 2 positions
        /// </summary>
        /// <param name="_pos1">Position 1</param>
        /// <param name="_pos2">Position 2</param>
        /// <returns></returns>
        public static dynamic GetRandomPosInArea(Vector3 _pos1, Vector3 _pos2)
        {
            // Get min and max X coords
            var minX = Math.Min(_pos1.X, _pos2.X);
            var maxX = Math.Max(_pos1.X, _pos2.X);

            // Get min and max Y coords
            var minY = Math.Min(_pos1.Y, _pos2.Y);
            var maxY = Math.Max(_pos1.Y, _pos2.Y);

            // Get min and max Z coords
            var minZ = Math.Min(_pos1.Z, _pos2.Z);
            var maxZ = Math.Max(_pos1.Z, _pos2.Z);

            // Generate random coords
            var x = RandomUtils.RandomFloat(minX, maxX);
            var y = RandomUtils.RandomFloat(minY, maxY);
            var z = RandomUtils.RandomFloat(minZ, maxZ);

            // Return vector
            return new Vector3(x, y, z);
        }
    }
}
