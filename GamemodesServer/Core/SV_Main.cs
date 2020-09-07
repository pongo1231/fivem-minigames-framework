using CitizenFX.Core;
using GamemodesServer.Utils;

namespace GamemodesServer.Core
{
    /// <summary>
    /// Main class
    /// </summary>
    public class Main : GmScript
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Main()
        {
            OnServerResourceStart();
        }

        /// <summary>
        /// Player dropped function
        /// </summary>
        /// <param name="_player">Player</param>
        /// <param name="_dropReason">Reason for drop</param>
        [PlayerDropped]
        private void OnPlayerDropped(Player _player, string _dropReason)
        {
            Log.WriteLine($"Dropped player {_player.Name} ({_dropReason})!");
        }

        /// <summary>
        /// Called when script has started
        /// </summary>
        private void OnServerResourceStart()
        {

        }
    }
}