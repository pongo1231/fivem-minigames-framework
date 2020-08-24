namespace Shared
{
    public struct GmProp
    {
        public GmProp(string _propName, dynamic _propPos, dynamic _propRot, bool _propCollisions)
        {
            PropName = _propName;
            PropPos = _propPos;
            PropRot = _propRot;
            PropCollisions = _propCollisions;
        }

        public string PropName { get; private set; }
        public dynamic PropPos { get; private set; }
        public dynamic PropRot { get; private set; }
        public bool PropCollisions { get; private set; }
    }
}
