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
        /// Time hours
        /// </summary>
        private static int s_timeHour;

        /// <summary>
        /// Time minutes
        /// </summary>
        private static int s_timeMin;

        /// <summary>
        /// Time seconds
        /// </summary>
        private static int s_timeSec;

        /// <summary>
        /// Weather
        /// </summary>
        private static string s_weather = "EXTRASUNNY";
        
        /// <summary>
        /// Tick function
        /// </summary>
        [Tick]
        private async Task OnTick()
        {
            // Broadcast time and weather to all clients
            TriggerClientEvent("gamemodes:cl_sv_settimeweather", s_timeHour, s_timeMin, s_timeSec, s_weather);

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

        /// <summary>
        /// Set weather
        /// </summary>
        /// <param name="_weather">Weather name</param>
        public static void SetWeather(string _weather)
        {
            s_weather = _weather;
        }
    }
}
