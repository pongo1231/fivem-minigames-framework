using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Manager of local time and weather
    /// </summary>
    public class TimeWeatherManager : GmScript
    {
        /// <summary>
        /// Set time and weather event by server
        /// </summary>
        /// <param name="_h">Time hours</param>
        /// <param name="_m">Time minutes</param>
        /// <param name="_s">Time seconds</param>
        /// <param name="_weather">Weather</param>
        [EventHandler("gamemodes:cl_sv_settimeweather")]
        private void OnSetTimeWeather(int _h, int _m, int _s, string _weather)
        {
            // Set time
            API.NetworkOverrideClockTime(_h, _m, _s);

            // Set weather
            API.SetWeatherTypeNowPersist(_weather);
        }
    }
}
