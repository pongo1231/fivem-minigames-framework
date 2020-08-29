using CitizenFX.Core;
using System.Threading.Tasks;

namespace GamemodesServer.Core
{
    public class TimecycModManager : GmScript
    {
        private static string s_timecycMod;
        private static string s_timecycModExtra;

        [Tick]
        private async Task OnTick()
        {
            if (s_timecycMod != null)
            {
                TriggerClientEvent("gamemodes:cl_sv_loadtimecycmods", s_timecycMod, s_timecycModExtra);
            }

            await Delay(300);
        }

        public static void SetTimecycModifiers(string _mod1, string _mod2 = null)
        {
            s_timecycMod = _mod1;
            s_timecycModExtra = _mod2;
        }

        public static void ClearTimecycModifiers()
        {
            s_timecycMod = null;
            s_timecycModExtra = null;
        }
    }
}
