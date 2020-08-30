using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GamemodesServer.Core
{
    /// <summary>
    /// Entity guard class
    /// </summary>
    public class EntityGuard : GmScript
    {
        /// <summary>
        /// Whether we should allow networked entities to be created
        /// </summary>
        public static bool AllowThrough = false;

        /// <summary>
        /// Entity creating event
        /// </summary>
        /// <param name="_handle">Handle of entity</param>
        [EventHandler("entityCreating")]
        private void OnEntityCreating(int _handle)
        {
            // Block this entity if guard is active
            if (!AllowThrough)
            {
                API.CancelEvent();
            }
        }
    }
}
