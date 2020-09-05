using CitizenFX.Core;
using GamemodesServer.Core.Map;
using System.Collections.Generic;

namespace GamemodesServer.Gamemodes.Hoops
{
    public class Hoops_Map : GamemodeMap
    {
        public class Hoop
        {
            public Hoop(Vector3 _position, Vector3 _rotation, bool _isExtraWorth = false)
            {
                IsActive = true;
                RespawnTimestamp = 0;
                Position = _position;
                Rotation = _rotation;
                IsExtraWorth = _isExtraWorth;
            }

            public bool IsActive;
            public long RespawnTimestamp;
            public Vector3 Position { get; private set; }
            public Vector3 Rotation { get; private set; }
            public bool IsExtraWorth { get; private set; }
        }

        public float FallOffHeight { get; protected set; }

        public Hoop[] Hoops { get; protected set; }
    }
}
