using CitizenFX.Core;
using CitizenFX.Core.UI;
using System.Drawing;
using System.Threading.Tasks;
using Font = CitizenFX.Core.UI.Font;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Manager for shown scores
    /// </summary>
    public class ScoreManager : GmScript
    {
        /// <summary>
        /// Whether scores should be drawn on screen
        /// </summary>
        public static bool DrawScores = false;

        /// <summary>
        /// Scores text
        /// </summary>
        private Text m_goalsText = new Text(null, new PointF(640f, 50f), 1.5f,
            Color.FromArgb(255, 255, 255), Font.Pricedown, Alignment.Center, true, true);

        /// <summary>
        /// Update scores event by server
        /// </summary>
        /// <param name="_redScore">Red score</param>
        /// <param name="_blueScore">Blue score</param>
        [EventHandler("gamemodes:cl_sv_updatescores")]
        private void OnServerUpdateScores(int _redScore, int _blueScore)
        {
            // Set score text
            m_goalsText.Caption = $"~r~{_redScore}   ~b~{_blueScore}";

            // Respond to server
            TriggerServerEvent("gamemodes:sv_cl_gotscores");
        }

        /// <summary>
        /// Tick function
        /// </summary>
        [Tick]
        private async Task OnTick()
        {
            // Draw scores text if enabled (and not in demo mode)
            if (DrawScores && !DemoMode.IsInDemoMode)
            {
                m_goalsText.Draw();
            }

            await Task.FromResult(0);
        }
    }
}
