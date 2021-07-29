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
        [EventHandler("explosionEvent")]
        private void OnExplosionEvent([FromSource] Player _player, string _data)
        {
            // Cancel it, we don't need any networked explosions
            API.CancelEvent();
        }
    }
}
