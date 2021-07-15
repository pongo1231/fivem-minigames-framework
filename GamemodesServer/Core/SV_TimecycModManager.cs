using CitizenFX.Core;
using System.Threading.Tasks;

namespace GamemodesServer.Core
{
    /// <summary>
    /// Manager of timecycle for all players
    /// </summary>
    public class TimecycModManager : GmScript
    {
        /// <summary>
        /// Timecycle Modifier
        /// </summary>
        private static string s_timecycMod;

        /// <summary>
        /// Timecycle Extra Modifier
        /// </summary>
        private static string s_timecycModExtra;

        /// <summary>
        /// Tick function
        /// </summary>
        [Tick]
        private async Task OnTick()
        {
            // Broadcast timecycle modifier to all clients if existant
            if (s_timecycMod != null)
            {
                TriggerClientEvent("gamemodes:cl_sv_loadtimecycmods", s_timecycMod, s_timecycModExtra);
            }

            await Delay(300);
        }

        /// <summary>
        /// Set timecycle modifiers
        /// </summary>
        /// <param name="_mod1">Timecycle modifier</param>
        /// <param name="_mod2">Timecycle extra modifier</param>
        public static void SetTimecycModifiers(string _mod1, string _mod2 = null)
        {
            s_timecycMod = _mod1;
            s_timecycModExtra = _mod2;
        }

        /// <summary>
        /// Clear timecycle modifiers
        /// </summary>
        public static void ClearTimecycModifiers()
        {
            s_timecycMod = null;
            s_timecycModExtra = null;
        }
    }
}
