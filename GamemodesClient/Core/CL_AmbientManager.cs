using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;

namespace GamemodesClient.Core
{
    public class AmbientManager : BaseScript
    {
        [Tick]
        private async Task OnTick()
        {
            if (API.GetIsLoadingScreenActive() || !Game.PlayerPed.Exists())
            {
                return;
            }

            Game.Player.WantedLevel = 0;
            API.SetMaxWantedLevel(0);

            Game.PlayerPed.IsInvincible = true;

            API.SetAmbientPedRangeMultiplierThisFrame(0f);
            API.SetAmbientVehicleRangeMultiplierThisFrame(0f);
            API.SetParkedVehicleDensityMultiplierThisFrame(0f);
            API.SetPedDensityMultiplierThisFrame(0f);
            API.SetRandomVehicleDensityMultiplierThisFrame(0f);
            API.SetScenarioPedDensityMultiplierThisFrame(0f, 0f);
            API.SetSomeVehicleDensityMultiplierThisFrame(0f);
            API.SetVehicleDensityMultiplierThisFrame(0f);

            await Task.FromResult(0);
        }
    }
}
