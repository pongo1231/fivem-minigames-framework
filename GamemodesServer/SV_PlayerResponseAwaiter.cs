using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamemodesServer
{
    public class PlayerResponseAwaiter : GmScript
    {
        private static PlayerResponseAwaiter s_instance;

        public PlayerResponseAwaiter()
        {
            s_instance = this;
        }

        public static async Task AwaitResponse(Player _player, string _serverClientEventName, string _clientServerEventName, params object[] _serverClientEventArgs)
        {
            bool hasCompleted = false;

            Action<Player> callback = new Action<Player>((Player player) =>
            {
                Debug.WriteLine($"Got response for {_serverClientEventName}!");

                hasCompleted = true;
            });

            s_instance.EventHandlers[_clientServerEventName] += callback;

            _player.TriggerEvent(_serverClientEventName, _serverClientEventArgs);

            long lastTimeStamp = API.GetGameTimer();
            while (!hasCompleted)
            {
                long curTimeStamp = API.GetGameTimer();

                if (lastTimeStamp < curTimeStamp - 1000)
                {
                    if (!PlayerLoadStateManager.GetLoadedInPlayers().Contains(_player))
                    {
                        hasCompleted = true;
                    }

                    lastTimeStamp = curTimeStamp;

                    _player.TriggerEvent(_serverClientEventName, _serverClientEventArgs);
                }

                await Delay(0);
            }

            s_instance.EventHandlers[_clientServerEventName] -= callback;
        }
    }
}
