using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesClient.Utils;
using System;
using System.Threading.Tasks;

namespace GamemodesClient
{
    public class Main : BaseScript
    {
        public Main()
        {
            Tick += OnTick;
        }

        [EventHandler("onClientResourceStart")]
        private async void OnClientResourceStart(string _resourceName)
        {
            if (API.GetCurrentResourceName() != _resourceName)
            {
                return;
            }

            await ScreenUtils.FadeOut();

            while (API.GetIsLoadingScreenActive())
            {
                await Delay(0);
            }

            TriggerServerEvent("gamemodes:sv_cl_loadedin");
        }

        private async Task OnTick()
        {
            await Task.FromResult(0);
        }
    }
}