using CitizenFX.Core;
using CitizenFX.Core.UI;
using System;
using System.Drawing;
using System.Threading.Tasks;
using Font = CitizenFX.Core.UI.Font;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Timer manager class
    /// </summary>
    public class TimerManager : BaseScript
    {
        /// <summary>
        /// Timer text
        /// </summary>
        private Text m_text = new Text(null, new PointF(1200f, 550f), 1f, Color.FromArgb(255, 255, 255), Font.Pricedown, Alignment.Right, true, true);

        /// <summary>
        /// Timer seconds
        /// </summary>
        public static int SecondsLeft { get; private set; }

        /// <summary>
        /// Set timer event by server
        /// </summary>
        /// <param name="_secondsLeft">Timer</param>
        [EventHandler("gamemodes:cl_sv_updatetimer")]
        private void UpdateTimer(int _secondsLeft)
        {
            // Set timer
            SecondsLeft = _secondsLeft;

            // Display overtime text if timer is at -1
            if (_secondsLeft == -1)
            {
                m_text.Caption = "Overtime!";
                m_text.Color = Color.FromArgb(255, 60, 60);
            }
            else
            {
                // Set timer text to timer
                m_text.Caption = _secondsLeft <= 0 ? null : TimeSpan.FromSeconds(_secondsLeft).ToString(@"mm\:ss");

                // Set text color to white initially
                Color textColor = Color.FromArgb(255, 255, 255);

                if (_secondsLeft <= 10)
                {
                    // Set text color to red if at or below 10 seconds
                    textColor = Color.FromArgb(255, 60, 60);
                }
                else if (_secondsLeft <= 30)
                {
                    // Set text color to yellow if at or below 30 seconds
                    textColor = Color.FromArgb(255, 255, 60);
                }

                // Apply text color
                m_text.Color = textColor;
            }
        }

        /// <summary>
        /// Tick function
        /// </summary>
        [Tick]
        private async Task OnTick()
        {
            // Draw timer text
            m_text.Draw();

            await Task.FromResult(0);
        }
    }
}
