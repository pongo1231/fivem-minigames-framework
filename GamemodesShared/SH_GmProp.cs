namespace GamemodesShared
{
    /// <summary>
    /// Shared Prop struct
    /// </summary>
    public struct SHGmProp
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_propName">Name of prop</param>
        /// <param name="_propPos">Position of prop</param>
        /// <param name="_propRot">Rotation of prop</param>
        /// <param name="_propCollisions">Whether prop has collisions</param>
        public SHGmProp(string _propName, dynamic _propPos, dynamic _propRot, bool _propCollisions)
        {
            PropName = _propName;
            PropPos = _propPos;
            PropRot = _propRot;
            PropCollisions = _propCollisions;
        }

        /// <summary>
        /// Name of prop
        /// </summary>
        public string PropName { get; private set; }

        /// <summary>
        /// Position of prop
        /// </summary>
        public dynamic PropPos { get; private set; }

        /// <summary>
        /// Rotation of prop
        /// </summary>
        public dynamic PropRot { get; private set; }

        /// <summary>
        /// Whether prop has collisions
        /// </summary>
        public bool PropCollisions { get; private set; }
    }
}
