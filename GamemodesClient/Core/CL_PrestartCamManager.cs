using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Threading.Tasks;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Prestart cam manager
    /// </summary>
    public class PrestartCamManager : BaseScript
    {
        /// <summary>
        /// Whether prestart cam should be shown
        /// </summary>
        private bool m_showPrestartCam = false;

        /// <summary>
        /// Scaleform used in prestart cam
        /// </summary>
        private Scaleform m_prestartScaleform;

        /// <summary>
        /// Prestart cam start event by server
        /// </summary>
        /// <param name="_gamemodeName">Name of current gamemode</param>
        /// <param name="_gamemodeDescription">Description of current gamemode</param>
        [EventHandler("gamemodes:cl_sv_showprestartcam")]
        private async void OnShowWinnerCam(string _gamemodeName, string _gamemodeDescription)
        {
            // Show prestart cam
            m_showPrestartCam = true;

            // Hide radar
            Screen.Hud.IsRadarVisible = false;

            // Create new MP_CELEBRATION scaleform
            m_prestartScaleform = new Scaleform("MP_CELEBRATION");

            // Wait for scaleform to load
            while (!m_prestartScaleform.IsLoaded)
            {
                await Delay(0);
            }

            // Create a new stat wall for MP_CELEBRATION
            m_prestartScaleform.CallFunction("CREATE_STAT_WALL", "SUMMARY", "HUD_COLOUR_TECH_GREEN_VERY_DARK", 255);

            // Add objective text to stat wall
            m_prestartScaleform.CallFunction("ADD_OBJECTIVE_TO_WALL", "SUMMARY", $"~y~{_gamemodeName}", $"~g~{_gamemodeDescription}", true);

            // Set duration stat wall is shown
            m_prestartScaleform.CallFunction("SET_PAUSE_DURATION", 18f);

            // Show the stat wall
            m_prestartScaleform.CallFunction("SHOW_STAT_WALL", "SUMMARY");
        }

        /// <summary>
        /// Prestart cam stop event by server
        /// </summary>
        [EventHandler("gamemodes:cl_sv_hideprestartcam")]
        private void OnHideWinnerCam()
        {
            // Hide prestart cam
            m_showPrestartCam = false;

            // Set radar as visible
            Screen.Hud.IsRadarVisible = true;

            // Clear scaleform
            m_prestartScaleform.Dispose();
            m_prestartScaleform = null;
        }

        [Tick]
        private async Task OnTick()
        {
            // Check if prestart is shown and scaleform exists
            if (m_showPrestartCam && m_prestartScaleform != null)
            {
                // Hide notifications
                API.ThefeedHideThisFrame();

                // Render scaleform
                m_prestartScaleform.Render2D();
            }

            await Task.FromResult(0);
        }
    }
}
