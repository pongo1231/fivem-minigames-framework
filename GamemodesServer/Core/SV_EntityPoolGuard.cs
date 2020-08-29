using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GamemodesServer.Core
{
    public class EntityPoolGuard : GmScript
    {
        public static bool AllowThrough = false;

        [EventHandler("entityCreating")]
        private void OnEntityCreating(int _handle)
        {
            if (!AllowThrough)
            {
                API.CancelEvent();
            }
        }
    }
}
