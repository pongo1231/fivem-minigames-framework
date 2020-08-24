using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GamemodesClient.Utils
{
    public static class EntityUtils
    {
        public static void FadeIn(this Entity _entity)
        {
            API.NetworkFadeInEntity(_entity.Handle, false);
        }

        public static void FadeOut(this Entity _entity)
        {
            API.NetworkFadeOutEntity(_entity.Handle, true, false);
        }

        public static void RequestControl(this Entity _entity)
        {
            API.NetworkRequestControlOfNetworkId(_entity.NetworkId);
        }

        public static bool IsNetworked(this Entity _entity)
        {
            return API.NetworkGetEntityIsNetworked(_entity.Handle);
        }
    }
}
