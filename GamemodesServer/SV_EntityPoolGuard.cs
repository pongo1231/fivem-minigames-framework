using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesServer.Utils;

namespace GamemodesServer
{
    public class EntityPoolGuard : GmScript
    {
        public static bool AllowThrough = false;

        [EventHandler("entityCreating")]
        private void OnEntityCreating(int _handle)
        {
            if (!AllowThrough)
            {
                Log.WriteLine($"Dropping entity {_handle}");

                API.CancelEvent();
            }
        }
    }
}
