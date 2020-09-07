using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using GamemodesClient.Utils;
using GamemodesShared;
using System.Threading.Tasks;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Winner cam manager class
    /// </summary>
    public class WinnerCamManager : GmScript
    {
        /// <summary>
        /// Whether to show winner cam
        /// </summary>
        private bool m_showWinnerCam = false;

        /// <summary>
        /// Scaleform for winner cam
        /// </summary>
        private Scaleform m_winnerScaleform;

        /// <summary>
        /// Show winner cam event by server
        /// </summary>
        /// <param name="_winnerTeam">Winner team</param>
        [EventHandler("gamemodes:cl_sv_showwinnercam")]
        private async void OnShowWinnerCam(int _winnerTeam, int _redScore, int _blueScore)
        {
            // Show winner cam
            m_showWinnerCam = true;

            // Transition to blurred
            API.TransitionToBlurred(100f);

            // Play anim postfx
            API.AnimpostfxPlay("MP_Celeb_Win", 0, true);

            // Hide radar
            Screen.Hud.IsRadarVisible = false;

            // Pause game
            Game.Pause(true);

            // Initialize MP_CELEBRATION scaleform
            m_winnerScaleform = new Scaleform("MP_CELEBRATION");

            // Wait for scaleform to load
            while (!m_winnerScaleform.IsLoaded)
            {
                await Delay(0);
            }

            // Create winner text label for scaleform
            ETeamType winnerTeam = (ETeamType)_winnerTeam;

            switch (winnerTeam)
            {
                case ETeamType.TEAM_RED:
                    API.AddTextEntry("_GAMEMODES_WINNER", "~r~red~w~ won!");

                    break;
                case ETeamType.TEAM_BLUE:
                    API.AddTextEntry("_GAMEMODES_WINNER", "~b~blue~w~ won!");

                    break;
                case ETeamType.TEAM_UNK:
                    API.AddTextEntry("_GAMEMODES_WINNER", "a tie!");

                    break;
            }

            // Create MP_CELEBRATION stat wall
            m_winnerScaleform.CallFunction("CREATE_STAT_WALL", "SUMMARY", "HUD_COLOUR_FRIENDLY", 255);

            // Add winner text to stat wall
            m_winnerScaleform.CallFunction("ADD_WINNER_TO_WALL", "SUMMARY", "_GAMEMODES_WINNER", "", "", 0, false, $"~r~{_redScore}~w~ - ~b~{_blueScore}", true);

            // Add background to stat wall
            m_winnerScaleform.CallFunction("ADD_BACKGROUND_TO_WALL", "SUMMARY", 255, 0);

            // Set pause duration of stat wall
            m_winnerScaleform.CallFunction("SET_PAUSE_DURATION", 5f);

            // Show stat wall
            m_winnerScaleform.CallFunction("SHOW_STAT_WALL", "SUMMARY");
        }

        /// <summary>
        /// Hide winner cam event by server
        /// </summary>
        [EventHandler("gamemodes:cl_sv_hidewinnercam")]
        private void OnHideWinnerCam()
        {
            // Hide winner cam
            m_showWinnerCam = false;

            // Unblur screen
            API.TransitionFromBlurred(100f);

            // Stop anim postfx
            API.AnimpostfxStop("MP_Celeb_Win");

            // Show radar again
            Screen.Hud.IsRadarVisible = true;

            // Clean up and remove scaleform
            m_winnerScaleform.Dispose();
            m_winnerScaleform = null;

            // Unpause game
            Game.Pause(false);

            // Fade out screen
            _ = ScreenUtils.FadeOut();
        }

        /// <summary>
        /// Tick function
        /// </summary>
        [Tick]
        private async Task OnTick()
        {
            // Check if winner cam should be shown and scaleform exists
            if (m_showWinnerCam && m_winnerScaleform != null)
            {
                // Hide notifications
                API.ThefeedHideThisFrame();

                // Render scaleform
                m_winnerScaleform.Render2D();
            }

            await Task.FromResult(0);
        }
    }
}
