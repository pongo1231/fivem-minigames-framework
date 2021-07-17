using CitizenFX.Core;
using GamemodesClientMenuBase.Menu;

namespace GamemodesClientPrivate.Core.MapEditor.Menus
{
    public class MapAboutMenu : GmUserMenu
    {
        public MapAboutMenu() : base("About")
        {

        }

        protected override void Tick()
        {
            AddLabelItem("Gamemode & Mappy by pongo1231", (_idx, _label) =>
            {
                World.AddExplosion(Game.PlayerPed.Position, ExplosionType.Steam, 10f, 5f);
            });
        }
    }
}
