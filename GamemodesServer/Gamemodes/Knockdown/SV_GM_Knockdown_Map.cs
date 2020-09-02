using GamemodesServer.Core.Map;

namespace GamemodesServer.Gamemodes.Knockdown
{
    public abstract class Knockdown_Map : GamemodeMap
    {
        public float FallOffHeight { get; protected set; }
    }
}
