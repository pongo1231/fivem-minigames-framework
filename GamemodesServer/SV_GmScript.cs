using CitizenFX.Core;

namespace GamemodesServer
{
    public class GmScript : BaseScript
    {
        public delegate void NewPlayerHandler(Player _player);
        public static NewPlayerHandler NewPlayer;

        public delegate void PlayerDroppedHandler(Player _player);
        public static PlayerDroppedHandler PlayerDropped;
    }
}
