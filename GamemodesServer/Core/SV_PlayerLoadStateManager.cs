using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamemodesServer.Core
{
    /// <summary>
    /// Player load state manager class
    /// </summary>
    public class PlayerLoadStateManager : GmScript
    {
        /// <summary>
        /// List of players
        /// </summary>
        private static List<Player> s_loadedPlayers = new List<Player>();

        /// <summary>
        /// Dropped player function
        /// </summary>
        /// <param name="_player">Player</param>
        /// <param name="_dropReason">Reason for drop</param>
        [PlayerDropped]
        private void OnPlayerDropped(Player _player, string _dropReason)
        {
            // Remove player from list
            s_loadedPlayers.Remove(_player);
        }

        /// <summary>
        /// Player loaded in event by client
        /// </summary>
        /// <param name="_player">Player</param>
        [EventHandler("gamemodes:sv_cl_loadedin")]
        private void OnClientLoadedIn([FromSource] Player _player)
        {
            // Check if player not loaded in already
            if (!s_loadedPlayers.Contains(_player))
            {
                // Add player to list
                s_loadedPlayers.Add(_player);

                // Invoke new player event
                NewPlayer?.Invoke(_player);
            }
        }

        /// <summary>
        /// Player dropped event
        /// </summary>
        /// <param name="_player">Player</param>
        /// <param name="_dropReason">Reason for drop</param>
        [EventHandler("playerDropped")]
        private void OnEventPlayerDropped([FromSource]Player _player, string _dropReason)
        {
            // Remove player from list
            s_loadedPlayers.Remove(_player);

            // Invoke player dropped event
            PlayerDropped?.Invoke(_player, _dropReason);
        }

        /// <summary>
        /// Tick function
        /// </summary>
        [Tick]
        private async Task OnTick()
        {
            // Iterate through all saved players
            foreach (Player player in s_loadedPlayers.ToArray())
            {
                // Check if player not ingame anymore
                if (!Players.Contains(player))
                {
                    // Remove player from list
                    s_loadedPlayers.Remove(player);

                    // Invoke player dropped event
                    PlayerDropped?.Invoke(player, "playerDropped not called");
                }
            }

            await Delay(100);
        }

        /// <summary>
        /// Check whether player has loaded in
        /// </summary>
        /// <param name="_player">Player</param>
        /// <returns>Whether they have loaded in</returns>
        public static bool HasLoadedIn(Player _player)
        {
            return s_loadedPlayers.Contains(_player);
        }

        /// <summary>
        /// Get all loaded in players
        /// </summary>
        /// <returns>Array of loaded in players</returns>
        public static Player[] GetLoadedInPlayers()
        {
            return s_loadedPlayers.ToArray();
        }
    }
}
