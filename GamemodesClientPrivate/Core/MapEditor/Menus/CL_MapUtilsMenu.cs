using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesClientMenuFw.GmMenuFw.Menu;

namespace GamemodesClientPrivate.Core.MapEditor.Menus
{
    public class MapUtilsMenu : GmUserMenu
    {
        public MapUtilsMenu() : base("Handy Utils")
        {
            ImmediateMode = false;

            AddActionItem("Spawn Bati", async (_idx, _label) =>
            {
                Model batiModel = VehicleHash.Bati;
                batiModel.Request();
                while (!batiModel.IsLoaded)
                {
                    await BaseScript.Delay(0);
                }

                var playerPos = Game.PlayerPed.Position;
                var veh = new Vehicle(API.CreateVehicle((uint)batiModel.Hash,
                    playerPos.X, playerPos.Y, playerPos.Z, Game.PlayerPed.Heading, false, false));
                veh.IsInvincible = true;

                Game.PlayerPed.SetIntoVehicle(veh, VehicleSeat.Driver);

                veh.MarkAsNoLongerNeeded();
                batiModel.MarkAsNoLongerNeeded();
            });
        }
    }
}
