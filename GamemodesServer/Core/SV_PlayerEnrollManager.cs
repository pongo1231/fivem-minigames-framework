using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamemodesServer.Core
{
    /// <summary>
    /// Manager for player enrolling to gamemodes
    /// </summary>
    public class PlayerEnrollStateManager : GmScript
    {
        /// <summary>
        /// List of players
        /// </summary>
        private static List<Player> s_enrolledPlayers = new List<Player>();

        /// <summary>
        /// Dropped player function
        /// </summary>
        /// <param name="_player">Player</param>
        /// <param name="_dropReason">Reason for drop</param>
        [PlayerDropped]
        private void OnPlayerDropped(Player _player, string _dropReason)
        {
            // Remove player from list
            s_enrolledPlayers.Remove(_player);
        }

        /// <summary>
        /// Player requests roll in to gamemodes event by client
        /// </summary>
        /// <param name="_player">Player</param>
        [EventHandler("gamemodes:sv_cl_request_roll_in")]
        private async void OnClientRequestRollIn([FromSource] Player _player)
        {
            // Check if auto enroll is turned on and player isn't already enrolled
            bool disableAutoEnroll = API.GetConvarInt("gamemodes_disable_auto_enroll", 0) != 0;
            if (disableAutoEnroll || s_enrolledPlayers.Contains(_player))
            {
                return;
            }

            await PlayerResponseAwaiter.AwaitResponse(_player, "gamemodes:cl_sv_accepted_roll_in", "gamemodes:sv_cl_prepared_for_roll_in");

            // Check if player not loaded in already
            if (!s_enrolledPlayers.Contains(_player))
            {
                // Add player to list
                s_enrolledPlayers.Add(_player);

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
            if (!s_enrolledPlayers.Contains(_player))
            {
                return;
            }

            // Remove player from list
            s_enrolledPlayers.Remove(_player);

            // Invoke player dropped event
            PlayerDropped?.Invoke(_player, _dropReason);
        }

        /// <summary>
        /// Tick function
        /// </summary>
        [Tick]
        private async Task OnTick()
        {
            bool disableAutoEnroll = API.GetConvarInt("gamemodes_disable_auto_enroll", 0) != 0;

            // Iterate through all saved players
            foreach (Player player in s_enrolledPlayers.ToArray())
            {
                // Check if player not ingame anymore
                if (disableAutoEnroll || !Players.Contains(player))
                {
                    // Remove player from list
                    s_enrolledPlayers.Remove(player);

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
            return s_enrolledPlayers.Contains(_player);
        }

        /// <summary>
        /// Get all loaded in players
        /// </summary>
        /// <returns>Array of loaded in players</returns>
        public static Player[] GetLoadedInPlayers()
        {
            return s_enrolledPlayers.ToArray();
        }
    }
}
