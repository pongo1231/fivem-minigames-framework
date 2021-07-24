using CitizenFX.Core;
using GamemodesServer.Core.Map;

namespace GamemodesServer.Gamemodes.Hoops
{
    /// <summary>
    /// Hoops map base class
    /// </summary>
    public class Hoops_Map : GamemodeBaseMap
    {
        /// <summary>
        /// Hoop class
        /// </summary>
        public class Hoop
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="_x">X coordinate</param>
            /// <param name="_y">Y coordinate</param>
            /// <param name="_z">Z coordinate</param>
            /// <param name="_isExtraWorth">If hoop gives extra points
            /// (for ones in hard to reach places)</param>
            public Hoop(float _x, float _y, float _z, bool _isExtraWorth = false)
            {
                Position = new Vector3(_x, _y, _z);
                IsExtraWorth = _isExtraWorth;
            }

            /// <summary>
            /// If hoop is active
            /// </summary>
            public bool IsActive = true;

            /// <summary>
            /// Time when the hoop should respawn again
            /// </summary>
            public long RespawnTimestamp = 0;

            /// <summary>
            /// Position of hoop
            /// </summary>
            public Vector3 Position { get; private set; }

            /// <summary>
            /// If hoop should give extra points
            /// </summary>
            public bool IsExtraWorth { get; private set; }
        }

        /// <summary>
        /// Array of hoops
        /// </summary>
        public Hoop[] Hoops { get; protected set; }
    }
}
