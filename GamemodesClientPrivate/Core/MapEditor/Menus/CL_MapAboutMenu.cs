using CitizenFX.Core;
using GamemodesClientMenuFw.GmMenuFw.Menu;

namespace GamemodesClientPrivate.Core.MapEditor.Menus
{
    /// <summary>
    /// About map editor menu
    /// </summary>
    public class MapAboutMenu : GmUserMenu
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MapAboutMenu() : base("About")
        {

        }

        /// <summary>
        /// Tick function
        /// </summary>
        protected override void Tick()
        {
            AddActionItem("Gamemode & Mappy by pongo1231", (_idx, _label) =>
            {
                World.AddExplosion(Game.PlayerPed.Position, ExplosionType.Steam, 10f, 5f);
            });

            AddLabelItem($"lol ur {Game.GameTime}");
        }
    }
}
