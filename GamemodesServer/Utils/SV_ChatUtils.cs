using CitizenFX.Core;
using GamemodesServer.Core;

namespace GamemodesServer.Utils
{
    /// <summary>
    /// Utils for chat
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
            foreach (var player in PlayerEnrollStateManager.GetLoadedInPlayers())
            {
                player.SendMessage(_message);
            }
        }
    }
}
