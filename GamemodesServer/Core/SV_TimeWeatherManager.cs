using CitizenFX.Core;
using System.Threading.Tasks;

namespace GamemodesServer.Core
{
    public class TimeWeatherManager : GmScript
    {
        private static int s_timeHour;
        private static int s_timeMin;
        private static int s_timeSec;
        private static string s_weather = "EXTRASUNNY";
        
        [Tick]
        private async Task OnTick()
        {
            TriggerClientEvent("gamemodes:cl_sv_settimeweather", s_timeHour, s_timeMin, s_timeSec, s_weather);

            await Delay(300);
        }

        public static void SetTime(int _hour, int _min, int _sec)
        {
            s_timeHour = _hour;
            s_timeMin = _min;
            s_timeSec = _sec;
        }

        public static void SetWeather(string _weather)
        {
            s_weather = _weather;
        }
    }
}
