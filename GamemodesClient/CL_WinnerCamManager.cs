using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using GamemodesClient.Utils;
using System.Threading.Tasks;

namespace GamemodesClient
{
    public class WinnerCamManager : BaseScript
    {
        private bool m_showWinnerCam = false;

        private Scaleform m_winnerScaleform;

        [EventHandler("gamemodes:cl_sv_showwinnercam")]
        private void OnShowWinnerCam()
        {
            m_showWinnerCam = true;

            API.TransitionToBlurred(100f);
            API.AnimpostfxPlay("MP_Celeb_Win", 0, true);

            Screen.Hud.IsRadarVisible = false;

            m_winnerScaleform = new Scaleform("MP_CELEBRATION");
            //m_winnerScaleform.CallFunction("ADD_OBJECTIVE_TO_WALL", "SUMMARY", "Match Over", "Trolololol", true);

            Game.Pause(true);
        }

        [EventHandler("gamemodes:cl_sv_hidewinnercam")]
        private void OnHideWinnerCam()
        {
            m_showWinnerCam = false;

            API.TransitionFromBlurred(100f);
            API.AnimpostfxStop("MP_Celeb_Win");
            API.AnimpostfxPlay("MP_Celeb_Win_Out", 0, false);

            Screen.Hud.IsRadarVisible = true;

            m_winnerScaleform.Dispose();
            m_winnerScaleform = null;

            Game.Pause(false);

            _ = ScreenUtils.FadeOut();
        }

        [Tick]
        private async Task OnTick()
        {
            if (m_showWinnerCam && m_winnerScaleform != null)
            {
                m_winnerScaleform.Render2D();
            }

            await Task.FromResult(0);
        }
    }
}
