using CitizenFX.Core;
using GamemodesClient.Utils;
using GamemodesShared;
using System.Drawing;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Scooter manager class
    /// </summary>
    public class PlayerScooterManager : GmScript
    {
        /// <summary>
        /// Current scooter
        /// </summary>
        public static GmNetEntity<Vehicle> CurrentScooter { get; private set; }

        /// <summary>
        /// Server spawned scooter event by server
        /// </summary>
        /// <param name="_networkId">Network id of scooter</param>
        [EventHandler("gamemodes:cl_sv_spawnedscooter")]
        private async void OnServerSpawnScooter(int _networkId)
        {
            // Get scooter entity from network id
            CurrentScooter = new GmNetEntity<Vehicle>(_networkId, true);

            // Wait for scooter to exist
            while (!CurrentScooter.Exists)
            {
                await Delay(0);
            }

            // Request control of scooter
            CurrentScooter.Entity.RequestControl();

            // Set some attributes for scooter
            CurrentScooter.Entity.EngineHealth = float.MaxValue;
            CurrentScooter.Entity.LockStatus = VehicleLockStatus.StickPlayerInside;
            CurrentScooter.Entity.EnginePowerMultiplier = 5f;
            CurrentScooter.Entity.EngineTorqueMultiplier = 5f;
            CurrentScooter.Entity.IsEngineRunning = true;

            // Set corresponding scooter color depending on team
            if (TeamManager.TeamType == ETeamType.TEAM_RED)
            {
                CurrentScooter.Entity.Mods.CustomPrimaryColor = Color.FromArgb(255, 0, 0);
                CurrentScooter.Entity.Mods.CustomSecondaryColor = Color.FromArgb(255, 0, 0);
            }
            else if (TeamManager.TeamType == ETeamType.TEAM_BLUE)
            {
                CurrentScooter.Entity.Mods.CustomPrimaryColor = Color.FromArgb(0, 0, 255);
                CurrentScooter.Entity.Mods.CustomSecondaryColor = Color.FromArgb(0, 0, 255);
            }

            // Networked fade in animation for scooter
            CurrentScooter.Entity.FadeIn();

            // Set scooter as boost vehicle
            BoostManager.BoostVehicle = CurrentScooter;

            // Fade in screen
            await ScreenUtils.FadeIn();
        }

        /// <summary>
        /// Request a scooter from server
        /// </summary>
        public static void Request()
        {
            TriggerServerEvent("gamemodes:sv_cl_requestscooter", SpawnManager.SpawnPos, SpawnManager.SpawnRot);
        }

        /// <summary>
        /// Cleanup current scooter
        /// </summary>
        public static void Cleanup()
        {
            if (CurrentScooter.Exists)
            {
                // Delete if one exists
                CurrentScooter.Entity.Delete();

                // Reset variable
                CurrentScooter = default;
            }
        }
    }
}
