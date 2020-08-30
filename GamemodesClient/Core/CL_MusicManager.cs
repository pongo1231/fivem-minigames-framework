using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Threading.Tasks;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Music event manager class
    /// </summary>
    public class MusicManager : BaseScript
    {
        /// <summary>
        /// Currently running music event
        /// </summary>
        private static string s_curMusicEvent;

        /// <summary>
        /// Tick function
        /// </summary>
        /// <returns></returns>
        [Tick]
        private async Task OnTick()
        {
            if (TimerManager.SecondsLeft == 30)
            {
                // Play 30s countdown music when only 30 seconds are left
                Play("FM_COUNTDOWN_30S");
            }
            else if (TimerManager.SecondsLeft == 1)
            {
                // Kill countdown music towards end
                Play("FM_COUNTDOWN_30S_KILL");
            }

            await Task.FromResult(0);
        }

        /// <summary>
        /// Play a music event
        /// </summary>
        /// <param name="_musicEvent">Name of music event to play</param>
        /// <param name="_extraMusicEvent">Additional music event to trigger to control intensity of normal music event</param>
        public static void Play(string _musicEvent = null, string _extraMusicEvent = null)
        {
            // Set currently running music event
            s_curMusicEvent = _musicEvent;

            // Save extra music event
            string extraMusicEvent = _extraMusicEvent;

            // Check if music event is set
            if (_musicEvent == null)
            {
                /* Randomly select a music event */

                Random random = new Random();
                int choice = random.Next(2);

                switch (choice)
                {
                    // Deadline
                    case 0:
                        s_curMusicEvent = "BKR_DEADLINE_START_MUSIC";
                        extraMusicEvent = "MP_MC_ACTION_HPREP";

                        break;

                    // Arena Wars
                    case 1:
                        choice = random.Next(1, 9);
                        s_curMusicEvent = $"MC_AW_MUSIC_{choice}";

                        break;
                }
            }

            // Trigger chosen music event
            API.TriggerMusicEvent(s_curMusicEvent);

            // Also trigger extra music event if one exists
            if (extraMusicEvent != null)
            {
                API.TriggerMusicEvent(extraMusicEvent);
            }
        }

        /// <summary>
        /// Stop currently running music event
        /// </summary>
        public static void Stop()
        {
            // Check if music event is running
            if (s_curMusicEvent != null)
            {
                // Cancel music event
                API.CancelMusicEvent(s_curMusicEvent);

                // Set music event as not running
                s_curMusicEvent = null;
            }
        }
    }
}
