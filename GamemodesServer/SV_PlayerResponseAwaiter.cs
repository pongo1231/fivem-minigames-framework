using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamemodesServer
{
    public class PlayerResponseAwaiter : BaseScript
    {
        private class PlayerResponseAwait
        {
            public PlayerResponseAwait(Player _player, string _serverClientEventName, string _clientServerEventName, params object[] _serverClientEventArgs)
            {
                Player = _player;
                ServerClientEventName = _serverClientEventName;
                ServerClientEventArgs = _serverClientEventArgs;
                ClientServerEventName = _clientServerEventName;
            }

            public Player Player { get; private set; }
            public string ServerClientEventName { get; private set; }
            public object[] ServerClientEventArgs { get; private set; }
            public string ClientServerEventName { get; private set; }
            public long LastTimeStamp = API.GetGameTimer();

            public delegate void Callback([FromSource]Player _player);
            public Callback ResponseCallback;
            public bool HasCompleted = false;
        }

        private static PlayerResponseAwaiter s_instance;
        private static List<PlayerResponseAwait> s_responseAwaits = new List<PlayerResponseAwait>();

        public PlayerResponseAwaiter()
        {
            s_instance = this;

            Tick += OnTick;
        }

        private async Task OnTick()
        {
            long curTimeStamp = API.GetGameTimer();

            foreach (PlayerResponseAwait responseAwait in s_responseAwaits)
            {
                if (responseAwait.LastTimeStamp < curTimeStamp - 1000)
                {
                    if (!Players.Contains(responseAwait.Player))
                    {
                        responseAwait.HasCompleted = true;
                    }
                    else
                    {
                        responseAwait.LastTimeStamp = curTimeStamp;

                        responseAwait.Player.TriggerEvent(responseAwait.ServerClientEventName, responseAwait.ServerClientEventArgs);
                    }
                }
            }

            await Task.FromResult(0);
        }

        public static async Task AwaitResponse(Player _player, string _serverClientEventName, string _clientServerEventName, params object[] _serverClientEventArgs)
        {
            PlayerResponseAwait responseAwait = new PlayerResponseAwait(_player, _serverClientEventName, _clientServerEventName, _serverClientEventArgs);

            responseAwait.ResponseCallback = (Player player) =>
            {
                Debug.WriteLine($"Got response for {_serverClientEventName}!");

                responseAwait.HasCompleted = true;

                s_instance.EventHandlers[_clientServerEventName] -= responseAwait.ResponseCallback;
            };

            s_responseAwaits.Add(responseAwait);

            s_instance.EventHandlers[_clientServerEventName] += new Action<Player>(responseAwait.ResponseCallback);

            _player.TriggerEvent(_serverClientEventName, _serverClientEventArgs);

            while (!responseAwait.HasCompleted)
            {
                await Delay(0);
            }

            s_responseAwaits.Remove(responseAwait);
        }
    }
}
