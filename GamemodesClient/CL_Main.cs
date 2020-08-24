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
        private void OnClientResourceStart(string _resourceName)
        {
            if (API.GetCurrentResourceName() != _resourceName)
            {
                return;
            }

            _ = ScreenUtils.FadeOut();
        }

        private async Task OnTick()
        {
            if (!API.GetIsLoadingScreenActive())
            {
                TriggerServerEvent("gamemodes:sv_cl_loadedin");
            }

            await Delay(500);
        }
    }
}