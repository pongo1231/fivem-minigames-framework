using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GamemodesClient.Core
{
    public class TimeWeatherManager : BaseScript
    {
        [EventHandler("gamemodes:cl_sv_settimeweather")]
        private void OnSetTimeWeather(int _h, int _m, int _s, string _weather)
        {
            API.NetworkOverrideClockTime(_h, _m, _s);
            API.SetWeatherTypeNowPersist(_weather);
        }
    }
}
