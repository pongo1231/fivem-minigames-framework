using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using GamemodesClient.Utils;
using GamemodesShared;
using System.Threading.Tasks;

namespace GamemodesClient
{
    public class WinnerCamManager : BaseScript
    {
        private bool m_showWinnerCam = false;

        private Scaleform m_winnerScaleform;

        [EventHandler("gamemodes:cl_sv_showwinnercam")]
        private async void OnShowWinnerCam(int _winnerTeam)
        {
            m_showWinnerCam = true;

            API.TransitionToBlurred(100f);
            API.AnimpostfxPlay("MP_Celeb_Win", 0, true);

            Screen.Hud.IsRadarVisible = false;

            Game.Pause(true);

            m_winnerScaleform = new Scaleform("MP_CELEBRATION");

            while (!m_winnerScaleform.IsLoaded)
            {
                await Delay(0);
            }

            m_winnerScaleform.CallFunction("CREATE_STAT_WALL", "SUMMARY", "HUD_COLOUR_FRIENDLY", 255);

            m_winnerScaleform.CallFunction("ADD_WINNER_TO_WALL", "SUMMARY", "CELEB_MATCH", "", "", 0, false,
                ((EPlayerTeamType)_winnerTeam) == EPlayerTeamType.TEAM_RED ? "~r~Team Red~w~ won!" : "~b~Team Blue~w~ won!", true);

            m_winnerScaleform.CallFunction("ADD_BACKGROUND_TO_WALL", "SUMMARY", 255, 0);

            m_winnerScaleform.CallFunction("SET_PAUSE_DURATION", 5f);

            m_winnerScaleform.CallFunction("SHOW_STAT_WALL", "SUMMARY");
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
                API.ThefeedHideThisFrame();

                m_winnerScaleform.Render2D();
            }

            await Task.FromResult(0);
        }
    }
}
