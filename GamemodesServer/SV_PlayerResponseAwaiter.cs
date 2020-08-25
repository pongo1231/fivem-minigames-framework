using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
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

            int timeoutTries = 3;

            Action<Player> callback = new Action<Player>((Player player) =>
            {
                Debug.WriteLine($"Got response from client for {_serverClientEventName}!");

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
                    else if (--timeoutTries == 0)
                    {
                        _player.Drop("Timed out during event handling");

                        return;
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
