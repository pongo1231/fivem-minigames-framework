using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System.Threading.Tasks;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Manager for all states that should be set at all times
    /// </summary>
    public class AmbientManager : GmScript
    {
        /// <summary>
        /// Tick function
        /// </summary>
        [Tick]
        private async Task OnTick()
        {
            // Abort if not fully loaded in yet
            if (API.GetIsLoadingScreenActive() || !Game.PlayerPed.Exists())
            {
                return;
            }

            // We don't need wanted levels in our gamemodes ever
            Game.Player.WantedLevel = 0;
            API.SetMaxWantedLevel(0);

            // Player never will need to die either (for now)
            Game.PlayerPed.IsInvincible = true;

            // No ped and traffic spawns to reduce load on clients
            API.SetAmbientPedRangeMultiplierThisFrame(0f);
            API.SetAmbientVehicleRangeMultiplierThisFrame(0f);
            API.SetParkedVehicleDensityMultiplierThisFrame(0f);
            API.SetPedDensityMultiplierThisFrame(0f);
            API.SetRandomVehicleDensityMultiplierThisFrame(0f);
            API.SetScenarioPedDensityMultiplierThisFrame(0f, 0f);
            API.SetSomeVehicleDensityMultiplierThisFrame(0f);
            API.SetVehicleDensityMultiplierThisFrame(0f);

            // Hide bottom right text (vehicle and street names)
            Screen.Hud.HideComponentThisFrame(HudComponent.VehicleName);
            Screen.Hud.HideComponentThisFrame(HudComponent.AreaName);
            Screen.Hud.HideComponentThisFrame(HudComponent.StreetName);

            // Disable handbrakes
            Game.DisableControlThisFrame(1, Control.VehicleHandbrake);

            // Disable cinematic camera (if not in demo mode)
            if (!DemoMode.IsInDemoMode)
            {
                Game.DisableControlThisFrame(1, Control.VehicleCinCam);
            }

            await Task.FromResult(0);
        }
    }
}
