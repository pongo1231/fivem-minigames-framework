using CitizenFX.Core;
using System.Threading.Tasks;

namespace GamemodesServer.Core
{
    /// <summary>
    /// Manager of timer for all players
    /// </summary>
    public class TimerManager : GmScript
    {
        /// <summary>
        /// Timer
        /// </summary>
        private static int s_secondsLeft;

        /// <summary>
        /// Whether we are in overtime (time == -1)
        /// </summary>
        public static bool InOvertime
        {
            get
            {
                return s_secondsLeft == -1;
            }
        }

        /// <summary>
        /// Whether timer has run out
        /// </summary>
        public static bool HasRunOut
        {
            get
            {
                return s_secondsLeft == 0 || s_secondsLeft == -1;
            }
        }

        /// <summary>
        /// New player function
        /// </summary>
        [NewPlayer]
        private void OnNewPlayer(Player _player)
        {
            _ = PlayerResponseAwaiter.AwaitResponse(_player, "gamemodes:cl_sv_updatetimer",
                "gamemodes:sv_cl_gottimer", s_secondsLeft);
        }

        /// <summary>
        /// Tick function
        /// </summary>
        [Tick]
        private async Task OnTick()
        {
            // Decrement timer as long as ît's above 0
            if (!HasRunOut)
            {
                s_secondsLeft--;

                SendClientEvent();
            }

            await Delay(1000);
        }

        /// <summary>
        /// Set timer
        /// </summary>
        /// <param name="_seconds">Timer</param>
        public static void SetTimer(int _seconds)
        {
            s_secondsLeft = _seconds;

            SendClientEvent();
        }

        /// <summary>
        /// Set overtime
        /// </summary>
        public static void SetOvertime()
        {
            s_secondsLeft = -1;

            SendClientEvent();
        }

        /// <summary>
        /// Stop timer
        /// </summary>
        public static void StopTimer()
        {
            s_secondsLeft = 0;

            SendClientEvent();
        }

        /// <summary>
        /// Sends the client event
        /// </summary>
        private static void SendClientEvent()
        {
            _ = PlayerResponseAwaiter.AwaitResponse("gamemodes:cl_sv_updatetimer",
                    "gamemodes:sv_cl_gottimer", s_secondsLeft);
        }
    }
}