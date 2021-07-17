using GamemodesClientMenuBase.Menu;

namespace GamemodesClientPrivate.Core.MapEditor.Menus
{
    public class MapMainMenu : GmUserMenu
    {
        public MapMainMenu() : base("Mappy The Editor")
        {

        }

        protected override void Tick()
        {
            AddChildMenuItem("About", new MapAboutMenu());
        }
    }
}
