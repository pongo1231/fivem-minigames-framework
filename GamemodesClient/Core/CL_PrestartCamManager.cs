using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using GamemodesClient.Utils;
using GamemodesShared.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Manager for camera stuff before gamemode start
    /// </summary>
    public class PrestartCamManager : GmScript
    {
        /// <summary>
        /// Whether prestart cam should be shown
        /// </summary>
        private bool m_showPrestartCam = false;

        /// <summary>
        /// Scaleforms used in prestart cam
        /// </summary>
        private List<Scaleform> m_scaleforms = new List<Scaleform>();

        /// <summary>
        /// Prestart cam start event by server
        /// </summary>
        /// <param name="_gamemodeName">Name of current gamemode</param>
        /// <param name="_gamemodeDescription">Description of current gamemode</param>
        [EventHandler("gamemodes:cl_sv_showprestartcam")]
        private async void OnShowPrestartCam(string _gamemodeName, string _gamemodeDescription)
        {
            // Show prestart cam
            m_showPrestartCam = true;

            // Hide radar
            Screen.Hud.IsRadarVisible = false;

            // Add scaleforms
            m_scaleforms.Add(new Scaleform("MP_CELEBRATION_BG"));
            m_scaleforms.Add(new Scaleform("MP_CELEBRATION_FG"));
            m_scaleforms.Add(new Scaleform("MP_CELEBRATION"));

            // Wait for scaleforms to load
            foreach (var scaleform in m_scaleforms)
            {
                while (!scaleform.IsLoaded)
                {
                    await Delay(0);
                }
            }

            // Wait for screen to fully fade in
            while (ScreenUtils.IsFadedOut)
            {
                await Delay(0);
            }

            foreach (var scaleform in m_scaleforms)
            {
                // Create stat wall
                scaleform.CallFunction("CREATE_STAT_WALL", "SUMMARY", "HUD_COLOUR_BLACK", 255);

                // Add intro to stat wall
                scaleform.CallFunction("ADD_INTRO_TO_WALL", "SUMMARY", $"~y~{_gamemodeName}",
                    $"~g~{_gamemodeDescription}", "", "", "", 0, 0, "", true, "HUD_COLOUR_BLACK");

                // Add background to stat wall
                scaleform.CallFunction("ADD_BACKGROUND_TO_WALL", "SUMMARY", 75, 0);

                // Set duration stat wall is shown
                scaleform.CallFunction("SET_PAUSE_DURATION", 9);

                // Show the stat wall
                scaleform.CallFunction("SHOW_STAT_WALL", "SUMMARY");
            }

            // Play new round sound
            if (RandomUtils.RandomInt(0, 100) == 0)
            {
                AudioUtils.PlayFrontendAudio("DLC_VW_AS_Sounds", "Survival_Passed");
            }
            else
            {
                AudioUtils.PlayFrontendAudio("CELEBRATION_SOUNDSET", "Shard_Appear");
            }
        }

        /// <summary>
        /// Prestart cam stop event by server
        /// </summary>
        [EventHandler("gamemodes:cl_sv_hideprestartcam")]
        private void OnHidePrestartCam()
        {
            // Hide prestart cam
            m_showPrestartCam = false;

            // Set radar as visible
            Screen.Hud.IsRadarVisible = true;

            // Clear scaleforms
            foreach (var scaleform in m_scaleforms)
            {
                scaleform.Dispose();
            }

            // Clear list
            m_scaleforms.Clear();
        }

        [Tick]
        private async Task OnTick()
        {
            // Check if prestart is shown
            if (m_showPrestartCam)
            {
                // Hide notifications
                API.ThefeedHideThisFrame();

                // Render scaleforms
                foreach (var scaleform in m_scaleforms)
                {
                    scaleform.Render2D();
                }
            }

            await Task.FromResult(0);
        }
    }
}