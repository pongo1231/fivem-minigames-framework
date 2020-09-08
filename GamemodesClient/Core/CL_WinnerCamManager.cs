using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using GamemodesClient.Utils;
using GamemodesShared;
using System.Collections.Generic;
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
        /// Scaleforms for winner cam
        /// </summary>
        private List<Scaleform> m_scaleforms = new List<Scaleform>();

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

            // Add scaleforms
            m_scaleforms.Add(new Scaleform("MP_CELEBRATION_BG"));
            m_scaleforms.Add(new Scaleform("MP_CELEBRATION_FG"));
            m_scaleforms.Add(new Scaleform("MP_CELEBRATION"));

            // Wait for scaleforms to load
            foreach (Scaleform scaleform in m_scaleforms)
            {
                while (!scaleform.IsLoaded)
                {
                    await Delay(0);
                }
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

            foreach (Scaleform scaleform in m_scaleforms)
            {
                // Create stat wall
                scaleform.CallFunction("CREATE_STAT_WALL", "SUMMARY", "HUD_COLOUR_BLACK", 255);

                // Add winner text to stat wall
                scaleform.CallFunction("ADD_WINNER_TO_WALL", "SUMMARY", "_GAMEMODES_WINNER", "", "", 0, false, $"~r~{_redScore}~w~ - ~b~{_blueScore}", true);

                // Add background to stat wall
                scaleform.CallFunction("ADD_BACKGROUND_TO_WALL", "SUMMARY", 75, 0);

                // Set pause duration of stat wall
                scaleform.CallFunction("SET_PAUSE_DURATION", 8);

                // Show stat wall
                scaleform.CallFunction("SHOW_STAT_WALL", "SUMMARY");
            }
        }

        /// <summary>
        /// Hide winner cam event by server
        /// </summary>
        [EventHandler("gamemodes:cl_sv_hidewinnercam")]
        private async void OnHideWinnerCam()
        {
            // Hide winner cam
            m_showWinnerCam = false;

            // Clean up scaleforms
            foreach (Scaleform scaleform in m_scaleforms)
            {
                scaleform.Dispose();
            }

            // Clear scaleforms list
            m_scaleforms.Clear();

            // Fade out screen
            await ScreenUtils.FadeOut();

            // Unblur screen
            API.TransitionFromBlurred(0f);

            // Stop anim postfx
            API.AnimpostfxStop("MP_Celeb_Win");

            // Show radar again
            Screen.Hud.IsRadarVisible = true;

            // Unpause game
            Game.Pause(false);
        }

        /// <summary>
        /// Tick function
        /// </summary>
        [Tick]
        private async Task OnTick()
        {
            // Check if winner cam should be shown
            if (m_showWinnerCam)
            {
                // Hide notifications
                API.ThefeedHideThisFrame();

                // Show cinematic cam
                API.SetCinematicModeActive(true);

                // Render scaleforms
                foreach (Scaleform scaleform in m_scaleforms)
                {
                    scaleform.Render2D();
                }
            }

            await Task.FromResult(0);
        }
    }
}
