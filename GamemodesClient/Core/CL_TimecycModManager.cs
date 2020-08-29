using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GamemodesClient.Core
{
    public class TimecycModManager : BaseScript
    {
        [EventHandler("gamemodes:cl_sv_loadtimecycmods")]
        private void OnLoadTimecycMod(string _mod1, string _mod2)
        {
            if (_mod1 != null)
            {
                API.SetTimecycleModifier(_mod1);
            }

            if (_mod2 != null)
            {
                API.SetExtraTimecycleModifier(_mod2);
            }

            API.PushTimecycleModifier();
        }
    }
}
