using CitizenFX.Core;

namespace Shared
{
    public struct GmProp
    {
        public GmProp(string _propName, Vector3 _propPos, Vector3 _propRot, bool _propCollisions)
        {
            PropName = _propName;
            PropPos = _propPos;
            PropRot = _propRot;
            PropCollisions = _propCollisions;
        }

        public string PropName { get; private set; }
        public Vector3 PropPos { get; private set; }
        public Vector3 PropRot { get; private set; }
        public bool PropCollisions { get; private set; }
    }
}
