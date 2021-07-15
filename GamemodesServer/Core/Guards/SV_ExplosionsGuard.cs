using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GamemodesServer.Core.Guards
{
    /// <summary>
    /// Guard for unauthorized explosions
    /// </summary>
    public class ExplosionsGuard : GmScript
    {
        /// <summary>
        /// Explosion event
        /// </summary>
        /// <param name="_player">Player who caused the explosion</param>
        /// <param name="data">JSON data of explosion</param>
        [EventHandler("explosionEvent")]
        private void OnExplosionEvent([FromSource] Player _player, string data)
        {
            // Cancel it, we don't need any networked explosions
            API.CancelEvent();
        }
    }
}
