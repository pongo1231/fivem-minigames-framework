using CitizenFX.Core;
using GamemodesServer.Core;
using System.Drawing;

namespace GamemodesServer.Utils
{
    /// <summary>
    /// Chat utils class
    /// </summary>
    public static class ChatUtils
    {
        /// <summary>
        /// Send a chat message to player
        /// </summary>
        /// <param name="_player">Player</param>
        /// <param name="_message">Message</param>
        /// <param name="_color">Message color</param>
        public static void SendMessage(this Player _player, string _message)
        {
            _player.TriggerEvent("chat:addMessage", new
            {
                color = new[] { 255, 0, 0 },
                multiline = true,
                args = new[] { "SYSTEM", _message }
            });
        }

        /// <summary>
        /// Send a chat message to all players
        /// </summary>
        /// <param name="_message">Message</param>
        /// <param name="_color">Message color</param>
        public static void SendMessage(string _message)
        {
            foreach (Player player in PlayerLoadStateManager.GetLoadedInPlayers())
            {
                player.SendMessage(_message);
            }
        }
    }
}
