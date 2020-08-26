using CitizenFX.Core;
using CitizenFX.Core.Native;
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

            while (API.GetIsLoadingScreenActive() || Game.PlayerPed == null || !Game.PlayerPed.Exists())
            {
                await Delay(0);
            }

            _ = ScreenUtils.FadeOut();

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