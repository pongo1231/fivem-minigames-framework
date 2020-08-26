using CitizenFX.Core;
using CitizenFX.Core.UI;
using GamemodesClient.Utils;
using System.Drawing;
using System.Threading.Tasks;
using Font = CitizenFX.Core.UI.Font;

namespace GamemodesClient
{
    public class PrestartCountdownManager : BaseScript
    {
        private Text m_text = new Text(null, new PointF(640f, 100f), 3f, Color.FromArgb(60, 255, 60), Font.Pricedown, Alignment.Center, true, true);
        private int m_timer;

        [EventHandler("gamemodes:cl_sv_setcountdowntimer")]
        private void OnSetCountdownTimer(int _timer)
        {
            m_text.Caption = $"{_timer}";

            m_timer = _timer;

            if (_timer == 0)
            {
                AudioUtils.PlayFrontendAudio("DLC_AW_Frontend_Sounds", "Countdown_GO");
            }
            else
            {
                AudioUtils.PlayFrontendAudio("DLC_AW_Frontend_Sounds", "Countdown_1");
            }
        }

        [Tick]
        private async Task OnTick()
        {
            if (m_timer > 0)
            {
                m_text.Draw();
            }

            await Task.FromResult(0);
        }
    }
}
