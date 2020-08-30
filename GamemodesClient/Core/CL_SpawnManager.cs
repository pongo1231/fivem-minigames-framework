using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesClient.Utils;
using System;
using System.Threading.Tasks;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Spawn manager class
    /// </summary>
    public class SpawnManager : BaseScript
    {
        /// <summary>
        /// Spawn Position
        /// </summary>
        public static Vector3 SpawnPos { get; private set; }

        /// <summary>
        /// Spawn Rotation
        /// </summary>
        public static Vector3 SpawnRot { get; private set; }

        /// <summary>
        /// Set spawn event by server
        /// </summary>
        /// <param name="_spawnPos">Spawn Position</param>
        /// <param name="_spawnRot">Spawn Rotation</param>
        [EventHandler("gamemodes:cl_sv_setspawn")]
        private void OnSetSpawn(Vector3 _spawnPos, Vector3 _spawnRot)
        {
            // Set spawn position and rotation
            SpawnPos = _spawnPos;
            SpawnRot = _spawnRot;

            // Respond to server waiting for us
            TriggerServerEvent("gamemodes:sv_cl_gotspawn");
        }

        /// <summary>
        /// Respawn player on currently set spawn position with currently set rotation
        /// </summary>
        public static async Task Respawn()
        {
            // Fade out screen
            await ScreenUtils.FadeOut();

            // Get current vehicle
            Vehicle vehicle = Game.PlayerPed.CurrentVehicle;

            // Set target to player ped initially
            Entity target = Game.PlayerPed;

            // Set target to vehicle if it exists
            if (vehicle != null)
            {
                target = vehicle;
            }

            // Request control of target (just in case)
            target.RequestControl();

            // Set coords to spawn position if set
            if (SpawnPos != default)
            {
                target.Position = SpawnPos;
            }

            // Set rotation to spawn rotation if set
            if (SpawnRot != default)
            {
                target.Rotation = SpawnRot;
            }

            // Reset velocity
            target.Velocity = default;

            // Repair vehicle if target is one
            if (API.GetEntityType(target.Handle) == 2)
            {
                ((Vehicle)target).Repair();
            }

            // Networked fade in animation of entity
            target.FadeIn();

            // Fade in screen again
            await ScreenUtils.FadeIn();
        }
    }
}
