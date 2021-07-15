using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GamemodesClient.Utils
{
    /// <summary>
    /// Entity utils
    /// </summary>
    public static class EntityUtils
    {
        /// <summary>
        /// Do networked fade in animation for entity
        /// </summary>
        /// <param name="_entity">Entity</param>
        public static void FadeIn(this Entity _entity)
        {
            API.NetworkFadeInEntity(_entity.Handle, false);
        }

        /// <summary>
        /// Do networked fade out animation for entity
        /// </summary>
        /// <param name="_entity">Entity</param>
        public static void FadeOut(this Entity _entity)
        {
            API.NetworkFadeOutEntity(_entity.Handle, true, false);
        }

        /// <summary>
        /// Request control of entity
        /// </summary>
        /// <param name="_entity">Entity</param>
        public static void RequestControl(this Entity _entity)
        {
            API.NetworkRequestControlOfNetworkId(_entity.NetworkId);
        }

        /// <summary>
        /// Check if entity is networked
        /// </summary>
        /// <param name="_entity">Entity</param>
        /// <returns>Whether entity is networked or not</returns>
        public static bool IsNetworked(this Entity _entity)
        {
            return API.NetworkGetEntityIsNetworked(_entity.Handle);
        }
    }
}
