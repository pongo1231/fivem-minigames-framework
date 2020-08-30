using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GamemodesServer.Utils
{
    /// <summary>
    /// Entity utils class
    /// </summary>
    public static class EntityUtils
    {
        /// <summary>
        /// Check whether the entity exists (on the server side)
        /// </summary>
        /// <param name="_entity">Entity</param>
        /// <returns></returns>
        public static bool Exists(this Entity _entity)
        {
            return _entity != null && API.DoesEntityExist(_entity.Handle);
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="_entity">Entity</param>
        public static void Delete(this Entity _entity)
        {
            API.DeleteEntity(_entity.Handle);
        }
    }
}
