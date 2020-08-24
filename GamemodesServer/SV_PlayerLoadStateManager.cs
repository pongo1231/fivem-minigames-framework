using CitizenFX.Core;
using System.Collections.Generic;

namespace GamemodesServer
{
    public class PlayerLoadStateManager : GmScript
    {
        private static List<Player> s_loadedPlayers = new List<Player>();

        public PlayerLoadStateManager()
        {
            PlayerDropped += OnPlayerDropped;
        }

        private void OnPlayerDropped(Player _player)
        {
            s_loadedPlayers.Remove(_player);
        }

        [EventHandler("gamemodes:sv_cl_loadedin")]
        private void OnClientLoadedIn([FromSource]Player _player)
        {
            if (!s_loadedPlayers.Contains(_player))
            {
                s_loadedPlayers.Add(_player);
            }
        }

        public static bool HasLoadedIn(Player _player)
        {
            return s_loadedPlayers.Contains(_player);
        }

        public static Player[] GetLoadedInPlayers()
        {
            return s_loadedPlayers.ToArray();
        }
    }
}
