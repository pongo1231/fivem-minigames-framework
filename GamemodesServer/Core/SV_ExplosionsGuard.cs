using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GamemodesServer.Core
{
    public class ExplosionsGuard : GmScript
    {
        [EventHandler("explosionEvent")]
        private void OnExplosionEvent([FromSource] Player _player, string data)
        {
            API.CancelEvent();
        }
    }
}
