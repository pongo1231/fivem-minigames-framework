﻿using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using GamemodesClient.Utils;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Main class
    /// </summary>
    public class Main : GmScript
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Main()
        {
            OnClientResourceStart();
        }

        /// <summary>
        /// On start function
        /// </summary>
        private async void OnClientResourceStart()
        {
            // Wait for game to fully load in
            while (API.GetIsLoadingScreenActive() || !Game.PlayerPed.Exists())
            {
                await Delay(0);
            }

            // Add extra delay for safety
            await Delay(5000);

            // Fade out screen
            await ScreenUtils.FadeOut();

            // Unpause game if it was paused from previous resource restart
            Game.Pause(false);

            // Unblur screen
            API.TransitionFromBlurred(0f);

            // Stop all screen effects
            Screen.Effects.Stop();

            // Set radar as visible
            Screen.Hud.IsRadarVisible = true;

            // Reset camera
            API.RenderScriptCams(false, false, 0, false, false);

            // Indicate to server we fully loaded in and are ready for gamemodes
            TriggerServerEvent("gamemodes:sv_cl_loadedin");
        }
    }
}