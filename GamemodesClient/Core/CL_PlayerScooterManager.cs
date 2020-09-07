using CitizenFX.Core;
using GamemodesClient.Utils;
using GamemodesShared;
using System.Drawing;
using System.Threading.Tasks;

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
        /// If scooter has been respawned this frame
        /// </summary>
        public static bool RespawnedThisFrame { get; private set; } = false;

        /// <summary>
        /// Height at which scooters should respawn
        /// </summary>
        private float m_fallOffHeight = float.MaxValue;

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
            CurrentScooter.Entity.HealthFloat = float.MaxValue;
            CurrentScooter.Entity.EngineHealth = float.MaxValue;
            CurrentScooter.Entity.PetrolTankHealth = float.MaxValue;
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

            // Notify server we finally received the scooter
            TriggerServerEvent("gamemodes:sv_cl_gotscooter");

            // Fade in screen
            await ScreenUtils.FadeIn();
        }

        /// <summary>
        /// Update falloff height event by server
        /// </summary>
        /// <param name="_height">Falloff height</param>
        [EventHandler("gamemodes:cl_sv_setscooterfalloffheight")]
        private void OnSetFallOffHeight(float _height)
        {
            m_fallOffHeight = _height;
        }

        /// <summary>
        /// Tick function
        /// </summary>
        [Tick]
        private async Task OnTick()
        {
            // Flag as not respawned this frame
            RespawnedThisFrame = false;

            if (CurrentScooter.Exists)
            {
                // Check if scooter at falloff height or dead
                if (CurrentScooter.Entity.Position.Z < m_fallOffHeight || CurrentScooter.Entity.IsDead)
                {
                    // Respawn scooter
                    await SpawnManager.Respawn();

                    // Flag as respawned this frame
                    RespawnedThisFrame = true;
                }
            }

            await Task.FromResult(0);
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
