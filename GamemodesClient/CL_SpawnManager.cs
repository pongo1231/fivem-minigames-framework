using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesClient.Utils;
using System;
using System.Threading.Tasks;

namespace GamemodesClient.Gamemodes
{
    public class SpawnManager : BaseScript
    {
        public static Vector3 SpawnPos { get; private set; }
        public static Vector3 SpawnRot { get; private set; }

        [EventHandler("gamemodes:cl_sv_setspawn")]
        private void OnSetSpawn(Vector3 _spawnPos, Vector3 _spawnRot)
        {
            SpawnPos = _spawnPos;
            SpawnRot = _spawnRot;

            TriggerServerEvent("gamemodes:sv_cl_gotspawn");
        }

        public static async Task Respawn()
        {
            await ScreenUtils.FadeOut();

            Vehicle vehicle = Game.PlayerPed.CurrentVehicle;

            Entity target = Game.PlayerPed;

            if (target != null)
            {
                target = vehicle;
            }

            target.RequestControl();

            if (SpawnPos != default)
            {
                target.Position = SpawnPos;
            }

            if (SpawnRot != default)
            {
                target.Rotation = SpawnRot;
            }

            target.Velocity = default;

            if (API.GetEntityType(target.Handle) == 2)
            {
                ((Vehicle)target).Repair();
            }

            target.FadeIn();

            await ScreenUtils.FadeIn();
        }
    }
}
