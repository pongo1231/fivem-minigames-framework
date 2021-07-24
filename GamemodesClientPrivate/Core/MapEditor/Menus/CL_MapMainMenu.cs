using GamemodesClientMenuFw.GmMenuFw.Menu;

namespace GamemodesClientPrivate.Core.MapEditor.Menus
{
    /// <summary>
    /// Main map editor menu
    /// </summary>
    public class MapMainMenu : GmUserMenu
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MapMainMenu() : base("Mappy The Editor")
        {
            ImmediateMode = false;

            var menu = new MapAboutMenu();

            AddChildMenuItem("About", menu);
        }
    }
}
