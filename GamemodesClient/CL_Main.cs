using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using GamemodesClient.Utils;
using System;
using System.Threading.Tasks;

namespace GamemodesClient
{
    public class Main : BaseScript
    {
        [EventHandler("onClientResourceStart")]
        private async void OnClientResourceStart(string _resourceName)
        {
            if (API.GetCurrentResourceName() != _resourceName)
            {
                return;
            }

            while (API.GetIsLoadingScreenActive() || !Game.PlayerPed.Exists())
            {
                await Delay(0);
            }

            _ = ScreenUtils.FadeOut();

            Game.Pause(false);

            API.TransitionFromBlurred(0f);

            Screen.Effects.Stop();

            Screen.Hud.IsRadarVisible = true;

            await Delay(2000);

            TriggerServerEvent("gamemodes:sv_cl_loadedin");
        }

        [Tick]
        private async Task OnTick()
        {
            await Task.FromResult(0);
        }
    }
}