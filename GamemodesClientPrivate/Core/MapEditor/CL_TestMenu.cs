using CitizenFX.Core;
using GamemodesClient.Core;
using GamemodesClientMenuBase.Menu;
using System.Threading.Tasks;

namespace GamemodesClientPrivate.Core
{
    public class TestMenu : GmScript
    {
        private readonly GamemodeTitledUserMenu m_menu = new GamemodeTitledUserMenu("Test");

        [Tick]
        private async Task OnTick()
        {
            if (m_menu.ShouldToggle)
            {
                m_menu.Visible = !m_menu.Visible;
            }

            if (m_menu.Visible)
            {
                m_menu.AddLabelItem("Teeest");
                m_menu.AddLabelItem("Lol this is just ridiculous", (_idx, _label) =>
                {
                    Game.PlayerPed.Ragdoll(100);
                });
                m_menu.AddLabelItem("Exactly");
            }

            m_menu.Update();

            await Task.FromResult(0);
        }
    }
}
