using CitizenFX.Core;
using GamemodesServer.Core.Map;

namespace GamemodesServer.Gamemodes.Hoops
{
    public class Hoops_Map : GamemodeMap
    {
        public class Hoop
        {
            public Hoop(float _x, float _y, float _z, bool _isExtraWorth = false)
            {
                IsActive = true;
                RespawnTimestamp = 0;
                Position = new Vector3(_x, _y, _z);
                IsExtraWorth = _isExtraWorth;
            }

            public bool IsActive;
            public long RespawnTimestamp;
            public Vector3 Position { get; private set; }
            public bool IsExtraWorth { get; private set; }
        }

        public float FallOffHeight { get; protected set; }

        public Hoop[] Hoops { get; protected set; }
    }
}
