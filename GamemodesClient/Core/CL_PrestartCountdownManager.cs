using CitizenFX.Core;
using CitizenFX.Core.UI;
using GamemodesClient.Utils;
using System.Drawing;
using System.Threading.Tasks;
using Font = CitizenFX.Core.UI.Font;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Prestart countdown manager class
    /// </summary>
    public class PrestartCountdownManager : BaseScript
    {
        /// <summary>
        /// Countdown text
        /// </summary>
        private Text m_text = new Text(null, new PointF(640f, 100f), 3f, Color.FromArgb(60, 255, 60), Font.Pricedown, Alignment.Center, true, true);

        /// <summary>
        /// Timer
        /// </summary>
        private int m_timer;

        /// <summary>
        /// Set countdown timer event from server
        /// </summary>
        /// <param name="_timer">Timer</param>
        [EventHandler("gamemodes:cl_sv_setcountdowntimer")]
        private void OnSetCountdownTimer(int _timer)
        {
            // Set text to timer
            m_text.Caption = $"{_timer}";

            // Set timer
            m_timer = _timer;

            // Play countdown sounds
            if (_timer == 0)
            {
                AudioUtils.PlayFrontendAudio("DLC_AW_Frontend_Sounds", "Countdown_GO");
            }
            else
            {
                AudioUtils.PlayFrontendAudio("DLC_AW_Frontend_Sounds", "Countdown_1");
            }
        }

        /// <summary>
        /// Tick function
        /// </summary>
        [Tick]
        private async Task OnTick()
        {
            // Draw timer text if it's above 0
            if (m_timer > 0)
            {
                m_text.Draw();
            }

            await Task.FromResult(0);
        }
    }
}
