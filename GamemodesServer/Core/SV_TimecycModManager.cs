using CitizenFX.Core;

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
        private static string s_timecycMod = null;

        /// <summary>
        /// Timecycle Extra Modifier
        /// </summary>
        private static string s_timecycModExtra = null;

        /// <summary>
        /// New player function
        /// </summary>
        [NewPlayer]
        private void OnNewPlayer(Player _player)
        {
            _ = PlayerResponseAwaiter.AwaitResponse(_player, "gamemodes:cl_sv_loadtimecycmods",
                "gamemodes:sv_cl_gottimecycmods", s_timecycMod, s_timecycModExtra);
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

            _ = PlayerResponseAwaiter.AwaitResponse("gamemodes:cl_sv_loadtimecycmods",
                "gamemodes:sv_cl_gottimecycmods", s_timecycMod, s_timecycModExtra);
        }

        /// <summary>
        /// Clear timecycle modifiers
        /// </summary>
        public static void ClearTimecycModifiers()
        {
            s_timecycMod = null;
            s_timecycModExtra = null;

            _ = PlayerResponseAwaiter.AwaitResponse("gamemodes:cl_sv_loadtimecycmods",
                "gamemodes:sv_cl_gottimecycmods", s_timecycMod, s_timecycModExtra);
        }
    }
}
