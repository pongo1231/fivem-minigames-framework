using CitizenFX.Core;

namespace GamemodesServer.Utils
{
    /// <summary>
    /// Math utils class
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// Check whether target position is within position 1 and position 2
        /// </summary>
        /// <param name="_target">Target position</param>
        /// <param name="_pos1">Position 1</param>
        /// <param name="_pos2">Position 2</param>
        /// <returns></returns>
        public static bool IsInArea(this Vector3 _target, Vector3 _pos1, Vector3 _pos2)
        {
            return ((_target.X > _pos1.X && _target.X < _pos2.X) || (_target.X < _pos1.X && _target.X > _pos2.X))
                && ((_target.Y > _pos1.Y && _target.Y < _pos2.Y) || (_target.Y < _pos1.Y && _target.Y > _pos2.Y))
                && ((_target.Z > _pos1.Z && _target.Z < _pos2.Z) || (_target.Z < _pos1.Z && _target.Z > _pos2.Z));
        }
    }
}
