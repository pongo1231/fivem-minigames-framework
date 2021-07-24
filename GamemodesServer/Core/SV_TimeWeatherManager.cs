using CitizenFX.Core;
using System.Threading.Tasks;

namespace GamemodesServer.Core
{
    /// <summary>
    /// Manager for time and weather for every player
    /// </summary>
    public class TimeWeatherManager : GmScript
    {
        /// <summary>
        /// Weather
        /// </summary>
        public static string Weather { get; set; } = "EXTRASUNNY";

        /// <summary>
        /// Time hours
        /// </summary>
        private static int s_timeHour = 12;

        /// <summary>
        /// Time minutes
        /// </summary>
        private static int s_timeMin = 0;

        /// <summary>
        /// Time seconds
        /// </summary>
        private static int s_timeSec = 0;
        
        /// <summary>
        /// Tick function
        /// </summary>
        [Tick]
        private async Task OnTick()
        {
            // Broadcast time and weather to all clients
            TriggerClientEvent("gamemodes:cl_sv_settimeweather", s_timeHour, s_timeMin, s_timeSec, Weather);

            await Delay(300);
        }

        /// <summary>
        /// Set time
        /// </summary>
        /// <param name="_hour">Hour</param>
        /// <param name="_min">Minutes</param>
        /// <param name="_sec">Seconds</param>
        public static void SetTime(int _hour, int _min, int _sec)
        {
            s_timeHour = _hour;
            s_timeMin = _min;
            s_timeSec = _sec;
        }
    }
}
