using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using GamemodesClient.Utils;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Entry point of the gamemode on client join
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
            await Delay(500);

            // Fade in screen for safety
            await ScreenUtils.FadeIn();

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

            // Clear timecyc mods
            TimecycModManager.ClearTimecycMods();

            // Ask server for rollin
            TriggerServerEvent("gamemodes:sv_cl_request_roll_in");
        }

        [EventHandler("gamemodes:cl_sv_accepted_roll_in")]
        private async void OnServerAcceptedRollIn()
        {
            // Fade out screen
            await ScreenUtils.FadeOut();

            // Indicate to server we fully loaded in and are ready for gamemodes
            TriggerServerEvent("gamemodes:sv_cl_prepared_for_roll_in");
        }
    }
}