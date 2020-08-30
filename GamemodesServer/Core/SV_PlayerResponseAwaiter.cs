using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamemodesServer.Core
{
    /// <summary>
    /// Player response awaiter class
    /// </summary>
    public class PlayerResponseAwaiter : GmScript
    {
        /// <summary>
        /// Current instance
        /// </summary>
        private static PlayerResponseAwaiter s_instance;

        /// <summary>
        /// Constructor
        /// </summary>
        public PlayerResponseAwaiter()
        {
            // Save this instance
            s_instance = this;
        }

        /// <summary>
        /// Send event and wait for client to either respond or get dropped because of not responding
        /// </summary>
        /// <param name="_player">Player</param>
        /// <param name="_serverClientEventName">Name of event to send to client</param>
        /// <param name="_clientServerEventName">Name of event to receive from client</param>
        /// <param name="_serverClientEventArgs">Arguments for sending to client</param>
        private async Task _AwaitResponse(Player _player, string _serverClientEventName, string _clientServerEventName, params object[] _serverClientEventArgs)
        {
            // Store whether it completed yet
            bool hasCompleted = false;

            // Amount of tries before players gets booted
            int timeoutTries = 2;

            // Callback for event
            Action<Player> callback = (player) =>
            {
                Debug.WriteLine($"Got response from client for {_serverClientEventName}!");

                // Set as completed
                hasCompleted = true;
            };

            // Register callback
            EventHandlers[_clientServerEventName] += callback;

            // Trigger event
            _player.TriggerEvent(_serverClientEventName, _serverClientEventArgs);

            // Wait for client to respond
            long lastTimeStamp = API.GetGameTimer();
            while (!hasCompleted)
            {
                long curTimeStamp = API.GetGameTimer();

                // Check if it's been 5 seconds already
                if (lastTimeStamp < curTimeStamp - 5000)
                {
                    // Check whether client is still in the game
                    if (!PlayerLoadStateManager.GetLoadedInPlayers().Contains(_player))
                    {
                        // Set as completed if client is gone
                        hasCompleted = true;
                    }
                    else if (--timeoutTries == 0)
                    {
                        /* Drop player for not responding */

                        Debug.WriteLine($"Dropped {_player.Name} for not responding to {_serverClientEventName}!");

                        // Drop player
                        _player.Drop("Timed out during event handling");

                        // Abort
                        return;
                    }

                    // Set last tried timestamp to current timestamp
                    lastTimeStamp = curTimeStamp;

                    // Send event again
                    _player.TriggerEvent(_serverClientEventName, _serverClientEventArgs);
                }

                await Delay(0);
            }

            // Remove callback
            EventHandlers[_clientServerEventName] -= callback;
        }

        /// <summary>
        /// Send event and wait for client to either respond or get dropped because of not responding
        /// </summary>
        /// <param name="_player">Player</param>
        /// <param name="_serverClientEventName">Name of event to send to client</param>
        /// <param name="_clientServerEventName">Name of event to receive from client</param>
        /// <param name="_serverClientEventArgs">Arguments for sending to client</param>
        public static async Task AwaitResponse(Player _player, string _serverClientEventName, string _clientServerEventName, params object[] _serverClientEventArgs)
        {
            await s_instance._AwaitResponse(_player, _serverClientEventName, _clientServerEventName, _serverClientEventArgs);
        }

        /// <summary>
        /// Send event and wait for all clients to either respond or get dropped because of not responding
        /// </summary>
        /// <param name="_serverClientEventName">Name of event to send to client</param>
        /// <param name="_clientServerEventName">Name of event to receive from client</param>
        /// <param name="_serverClientEventArgs">Arguments for sending to client</param>
        public static async Task AwaitResponse(string _serverClientEventName, string _clientServerEventName, params object[] _serverClientEventArgs)
        {
            // Return if there are no loaded in players
            if (PlayerLoadStateManager.GetLoadedInPlayers().Count() == 0)
            {
                return;
            }

            // List of waiting tasks
            List<Task> responseAwaits = new List<Task>();

            // Add a waiting task for each player
            foreach (Player player in PlayerLoadStateManager.GetLoadedInPlayers())
            {
                responseAwaits.Add(AwaitResponse(player, _serverClientEventName, _clientServerEventName, _serverClientEventArgs));
            }

            // Wait for all the waiting tasks to complete
            while (responseAwaits.Where(responseAwait => !responseAwait.IsCompleted).Count() > 0)
            {
                await Delay(0);
            }
        }
    }
}
