using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GamemodesServer.Utils
{
    public static class EntityUtils
    {
        public static bool Exists(this Entity _entity)
        {
            return _entity != null && API.DoesEntityExist(_entity.Handle);
        }

        public static void Delete(this Entity _entity)
        {
            API.DeleteEntity(_entity.Handle);
        }
    }
}
