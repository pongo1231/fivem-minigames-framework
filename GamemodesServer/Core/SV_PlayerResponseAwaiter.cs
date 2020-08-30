using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamemodesServer.Core
{
    public class PlayerResponseAwaiter : GmScript
    {
        private static PlayerResponseAwaiter s_instance;

        public PlayerResponseAwaiter()
        {
            s_instance = this;
        }

        private async Task _AwaitResponse(Player _player, string _serverClientEventName, string _clientServerEventName, params object[] _serverClientEventArgs)
        {
            bool hasCompleted = false;

            int timeoutTries = 2;

            Action<Player> callback = (player) =>
            {
                Debug.WriteLine($"Got response from client for {_serverClientEventName}!");

                hasCompleted = true;
            };

            EventHandlers[_clientServerEventName] += callback;

            _player.TriggerEvent(_serverClientEventName, _serverClientEventArgs);

            long lastTimeStamp = API.GetGameTimer();
            while (!hasCompleted)
            {
                long curTimeStamp = API.GetGameTimer();

                if (lastTimeStamp < curTimeStamp - 5000)
                {
                    if (!PlayerLoadStateManager.GetLoadedInPlayers().Contains(_player))
                    {
                        hasCompleted = true;
                    }
                    else if (--timeoutTries == 0)
                    {
                        Debug.WriteLine($"Dropped {_player.Name} for not responding to {_serverClientEventName}!");

                        _player.Drop("Timed out during event handling");

                        return;
                    }

                    lastTimeStamp = curTimeStamp;

                    _player.TriggerEvent(_serverClientEventName, _serverClientEventArgs);
                }

                await Delay(0);
            }

            EventHandlers[_clientServerEventName] -= callback;
        }

        public static async Task AwaitResponse(Player _player, string _serverClientEventName, string _clientServerEventName, params object[] _serverClientEventArgs)
        {
            await s_instance._AwaitResponse(_player, _serverClientEventName, _clientServerEventName, _serverClientEventArgs);
        }

        public static async Task AwaitResponse(string _serverClientEventName, string _clientServerEventName, params object[] _serverClientEventArgs)
        {
            if (PlayerLoadStateManager.GetLoadedInPlayers().Count() == 0)
            {
                return;
            }

            List<Task> responseAwaits = new List<Task>();

            foreach (Player player in PlayerLoadStateManager.GetLoadedInPlayers())
            {
                responseAwaits.Add(AwaitResponse(player, _serverClientEventName, _clientServerEventName, _serverClientEventArgs));
            }

            while (responseAwaits.Where(responseAwait => !responseAwait.IsCompleted).Count() > 0)
            {
                await Delay(0);
            }
        }
    }
}
