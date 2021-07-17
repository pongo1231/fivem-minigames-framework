using CitizenFX.Core;
using GamemodesClient.Core;
using GamemodesClientPrivate.Core.MapEditor.Menus;
using System.Threading.Tasks;

namespace GamemodesClientPrivate.Core.MapEditor
{
    public class MapMenuManager : GmScript
    {
        MapMainMenu m_mainMenu = new MapMainMenu();

        [Tick]
        private async Task OnTick()
        {
            if (Game.IsControlJustPressed(0, Control.InteractionMenu))
            {
                m_mainMenu.Visible = !m_mainMenu.Visible;
            }

            m_mainMenu.Update();

            await Task.FromResult(0);
        }
    }
}
