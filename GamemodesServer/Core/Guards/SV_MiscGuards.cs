using CitizenFX.Core.Native;
using CitizenFX.Core;

namespace GamemodesServer.Core.Guards
{
    /// <summary>
    /// Guards with the purpose of immediately blocking certain events
    /// </summary>
    public class MiscGuards : GmScript
    {
        /// <summary>
        /// Explosion event
        /// </summary>
        [EventHandler("explosionEvent")]
        private void OnExplosionEvent([FromSource] Player _player, string _data)
        {
            API.CancelEvent();
        }

        /// <summary>
        /// Ptfx Event
        /// </summary>
        [EventHandler("ptFxEvent")]
        private void OnPtfxEvent([FromSource] Player _player, string _data)
        {
            API.CancelEvent();
        }

        /// <summary>
        /// Clear Ped Tasks Event
        /// </summary>
        [EventHandler("clearPedTasksEvent")]
        private void OnClearPedTasksEvent([FromSource] Player _player, string _data)
        {
            API.CancelEvent();
        }

        /// <summary>
        /// Give Weapon Event
        /// </summary>
        [EventHandler("giveWeaponEvent")]
        private void OnGiveWeaponsEvent([FromSource] Player _player, string _data)
        {
            API.CancelEvent();
        }

        /// <summary>
        /// Remove Weapon Event
        /// </summary>
        [EventHandler("removeWeaponEvent")]
        private void OnRemoveWeaponsEvent([FromSource] Player _player, string _data)
        {
            API.CancelEvent();
        }

        /// <summary>
        /// Remove All Weapons Event
        /// </summary>
        [EventHandler("removeAllWeaponsEvent")]
        private void OnRemoveAllWeaponsEvent([FromSource] Player _player, string _data)
        {
            API.CancelEvent();
        }

        /// <summary>
        /// Fire Event
        /// </summary>
        [EventHandler("fireEvent")]
        private void OnFireEvent([FromSource] Player _player, string _data)
        {
            API.CancelEvent();
        }

        /// <summary>
        /// Weapon Damage Event
        /// </summary>
        [EventHandler("weaponDamageEvent")]
        private void OnWeaponDamageEvent([FromSource] Player _player, string _data)
        {
            API.CancelEvent();
        }

        /// <summary>
        /// Vehicle Component Control Event
        /// </summary>
        [EventHandler("vehicleComponentControlEvent")]
        private void OnVehicleComponentControlEvent([FromSource] Player _player, string _data)
        {
            API.CancelEvent();
        }
    }
}
