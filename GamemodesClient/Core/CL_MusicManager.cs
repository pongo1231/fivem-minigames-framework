using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesShared.Utils;
using System.Threading.Tasks;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Manager for music events
    /// </summary>
    public class MusicManager : GmScript
    {
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

            await Task.FromResult(0);
        }

        /// <summary>
        /// Play a music event
        /// </summary>
        /// <param name="_musicEvent">Name of music event to play</param>
        /// <param name="_extraMusicEvent">Additional music event to trigger to control intensity
        /// of normal music event</param>
        public static void Play(string _musicEvent = null, string _extraMusicEvent = null)
        {
            // Check if music event is set
            if (_musicEvent == null)
            {
                /* Randomly select a music event */

                var choice = RandomUtils.RandomInt(0, 2);

                switch (choice)
                {
                    // Deadline
                    case 0:
                        _musicEvent = "BKR_DEADLINE_START_MUSIC";
                        _extraMusicEvent = "MP_MC_ACTION_HPREP";

                        break;

                    // Arena Wars
                    case 1:
                        choice = RandomUtils.RandomInt(1, 9);
                        _musicEvent = $"MC_AW_MUSIC_{choice}";

                        break;
                }
            }

            // Trigger chosen music event
            API.TriggerMusicEvent(_musicEvent);

            // Also trigger extra music event if one exists
            if (_extraMusicEvent != null)
            {
                API.TriggerMusicEvent(_extraMusicEvent);
            }
        }

        /// <summary>
        /// Stop currently running music event
        /// </summary>
        public static void Stop()
        {
            Play("FM_COUNTDOWN_30S_KILL");
        }
    }
}
