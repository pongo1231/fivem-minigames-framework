using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamemodesServer
{
    public class Main : BaseScript
    {
        private List<Player> m_players = new List<Player>();

        public Main()
        {
            Tick += OnTick;

            OnServerResourceStart();
        }

        [EventHandler("playerDropped")]
        private void OnPlayerDropped([FromSource]Player _player, string _reason)
        {
            m_players.Remove(_player);

            GmScript.PlayerDropped?.Invoke(_player);
        }

        private void OnServerResourceStart()
        {

        }

        private async Task OnTick()
        {
            foreach (Player player in PlayerLoadStateManager.GetLoadedInPlayers())
            {
                if (!m_players.Contains(player))
                {
                    m_players.Add(player);

                    GmScript.NewPlayer?.Invoke(player);
                }
            }

            await Task.FromResult(0);
        }
    }
}