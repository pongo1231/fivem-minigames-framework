using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Threading.Tasks;

namespace GamemodesClient.Core
{
    public class PrestartCamManager : BaseScript
    {
        private bool m_showPrestartCam = false;

        private Scaleform m_prestartScaleform;

        [EventHandler("gamemodes:cl_sv_showprestartcam")]
        private async void OnShowWinnerCam(string _gamemodeName, string _gamemodeDescription)
        {
            m_showPrestartCam = true;

            Screen.Hud.IsRadarVisible = false;

            m_prestartScaleform = new Scaleform("MP_CELEBRATION");

            while (!m_prestartScaleform.IsLoaded)
            {
                await Delay(0);
            }

            m_prestartScaleform.CallFunction("CREATE_STAT_WALL", "SUMMARY", "HUD_COLOUR_TECH_GREEN_VERY_DARK", 255);

            m_prestartScaleform.CallFunction("ADD_OBJECTIVE_TO_WALL", "SUMMARY", $"~y~{_gamemodeName}", $"~g~{_gamemodeDescription}", true);

            m_prestartScaleform.CallFunction("SET_PAUSE_DURATION", 12f);

            m_prestartScaleform.CallFunction("SHOW_STAT_WALL", "SUMMARY");
        }

        [EventHandler("gamemodes:cl_sv_hideprestartcam")]
        private void OnHideWinnerCam()
        {
            m_showPrestartCam = false;

            Screen.Hud.IsRadarVisible = true;

            m_prestartScaleform.Dispose();
            m_prestartScaleform = null;
        }

        [Tick]
        private async Task OnTick()
        {
            if (m_showPrestartCam && m_prestartScaleform != null)
            {
                API.ThefeedHideThisFrame();

                m_prestartScaleform.Render2D();
            }

            await Task.FromResult(0);
        }
    }
}
