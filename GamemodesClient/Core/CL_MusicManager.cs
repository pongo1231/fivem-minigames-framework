using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Threading.Tasks;

namespace GamemodesClient.Core
{
    public class MusicManager : BaseScript
    {
        private static string s_curMusicEvent;

        [Tick]
        private async Task OnTick()
        {
            if (TimerManager.SecondsLeft == 30)
            {
                Play("FM_COUNTDOWN_30S");
            }
            else if (TimerManager.SecondsLeft == 1)
            {
                Play("FM_COUNTDOWN_30S_KILL");
            }

            await Task.FromResult(0);
        }

        public static void Play(string _musicEvent = null, string _extraMusicEvent = null)
        {
            s_curMusicEvent = _musicEvent;
            string extraMusicEvent = _extraMusicEvent;

            if (_musicEvent == null)
            {
                Random random = new Random();
                int choice = random.Next(2);

                switch (choice)
                {
                    case 0:
                        s_curMusicEvent = "BKR_DEADLINE_START_MUSIC";
                        extraMusicEvent = "MP_MC_ACTION_HPREP";

                        break;

                    case 1:
                        choice = random.Next(1, 9);
                        s_curMusicEvent = $"MC_AW_MUSIC_{choice}";

                        break;
                }
            }

            API.TriggerMusicEvent(s_curMusicEvent);

            if (extraMusicEvent != null)
            {
                API.TriggerMusicEvent(extraMusicEvent);
            }
        }

        public static void Stop()
        {
            if (s_curMusicEvent != null)
            {
                API.CancelMusicEvent(s_curMusicEvent);

                s_curMusicEvent = null;
            }
        }
    }
}
