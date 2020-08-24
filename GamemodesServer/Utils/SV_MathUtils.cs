using CitizenFX.Core;

namespace GamemodesServer.Utils
{
    public static class MathUtils
    {
        public static bool IsInArea(this Vector3 _target, Vector3 _pos1, Vector3 _pos2)
        {
            return ((_target.X > _pos1.X && _target.X < _pos2.X) || (_target.X < _pos1.X && _target.X > _pos2.X))
                && ((_target.Y > _pos1.Y && _target.Y < _pos2.Y) || (_target.Y < _pos1.Y && _target.Y > _pos2.Y))
                && ((_target.Z > _pos1.Z && _target.Z < _pos2.Z) || (_target.Z < _pos1.Z && _target.Z > _pos2.Z));
        }
    }
}
