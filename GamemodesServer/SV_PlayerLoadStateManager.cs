using CitizenFX.Core;
using System;
using System.Collections.Generic;

namespace GamemodesServer
{
    public class PlayerLoadStateManager : GmScript
    {
        private static List<Player> s_loadedPlayers = new List<Player>();

        public PlayerLoadStateManager()
        {

        }

        [PlayerDropped]
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

                NewPlayer?.Invoke(_player);
            }
        }

        [EventHandler("playerDropped")]
        private void OnPlayerDropped([FromSource] Player _player, string _reason)
        {
            s_loadedPlayers.Remove(_player);

            PlayerDropped?.Invoke(_player);
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
