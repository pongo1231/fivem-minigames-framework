using CitizenFX.Core;
using CitizenFX.Core.UI;
using System;
using System.Drawing;
using System.Threading.Tasks;
using Font = CitizenFX.Core.UI.Font;

namespace GamemodesClient.Core
{
    public class TimerManager : BaseScript
    {
        private Text m_text = new Text(null, new PointF(1200f, 550f), 1f, Color.FromArgb(255, 255, 255), Font.Pricedown, Alignment.Right, true, true);

        public static int SecondsLeft { get; private set; }

        [EventHandler("gamemodes:cl_sv_updatetimer")]
        private void UpdateTimer(int _secondsLeft)
        {
            SecondsLeft = _secondsLeft;

            if (_secondsLeft == -1)
            {
                m_text.Caption = "Overtime!";
                m_text.Color = Color.FromArgb(255, 60, 60);
            }
            else
            {
                m_text.Caption = _secondsLeft <= 0 ? null : TimeSpan.FromSeconds(_secondsLeft).ToString(@"mm\:ss");

                Color textColor = Color.FromArgb(255, 255, 255);
                if (_secondsLeft <= 10)
                {
                    textColor = Color.FromArgb(255, 60, 60);
                }
                else if (_secondsLeft <= 30)
                {
                    textColor = Color.FromArgb(255, 255, 60);
                }

                m_text.Color = textColor;
            }
        }

        [Tick]
        private async Task OnTick()
        {
            m_text.Draw();

            await Task.FromResult(0);
        }
    }
}
