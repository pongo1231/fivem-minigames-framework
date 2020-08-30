using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using GamemodesClient.Utils;
using System;
using System.Threading.Tasks;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Main class
    /// </summary>
    public class Main : BaseScript
    {
        /// <summary>
        /// Resource start function
        /// </summary>
        /// <param name="_resourceName">Name of resource</param>
        [EventHandler("onClientResourceStart")]
        private async void OnClientResourceStart(string _resourceName)
        {
            // Abort if started resource isn't this one
            if (API.GetCurrentResourceName() != _resourceName)
            {
                return;
            }

            // Wait for game to fully load in
            while (API.GetIsLoadingScreenActive() || !Game.PlayerPed.Exists())
            {
                await Delay(0);
            }

            // Add extra delay for safety
            await Delay(2000);

            // Fade out screen
            _ = ScreenUtils.FadeOut();

            // Unpause game if it was paused from previous resource restart
            Game.Pause(false);

            // Unblur screen
            API.TransitionFromBlurred(0f);

            // Stop all screen effects
            Screen.Effects.Stop();

            // Set radar as visible
            Screen.Hud.IsRadarVisible = true;

            // Indicate to server we fully loaded in and are ready for gamemodes
            TriggerServerEvent("gamemodes:sv_cl_loadedin");
        }
    }
}