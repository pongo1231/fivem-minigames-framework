using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Manager for timecycle
    /// </summary>
    public class TimecycModManager : GmScript
    {
        /// <summary>
        /// Load timecycle modifiers event by server
        /// </summary>
        /// <param name="_mod1">Timecycle modifier</param>
        /// <param name="_mod2">Extra timecycle modifier</param>
        [EventHandler("gamemodes:cl_sv_loadtimecycmods")]
        private void OnLoadTimecycMod(string _mod1, string _mod2)
        {
            // Set timecycle modifier is set
            if (_mod1 != null)
            {
                API.SetTimecycleModifier(_mod1);
            }

            // Set extra timecycle modifier if set
            if (_mod2 != null)
            {
                API.SetExtraTimecycleModifier(_mod2);
            }

            // Apply timecycle modifiers
            API.PushTimecycleModifier();

            TriggerServerEvent("gamemodes:sv_cl_gottimecycmods");
        }

        /// <summary>
        /// Clear all timecycle modifiers
        /// </summary>
        public static void ClearTimecycMods()
        {
            API.ClearTimecycleModifier();
            API.ClearExtraTimecycleModifier();
        }
    }
}
