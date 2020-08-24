using CitizenFX.Core;
using System;

namespace GamemodesClient.Gamemodes
{
    public class SpawnManager : BaseScript
    {
        public static Vector3 SpawnPos { get; private set; }
        public static Vector3 SpawnRot { get; private set; }

        public SpawnManager()
        {

        }

        [EventHandler("gamemodes:cl_sv_setspawn")]
        private void OnSetSpawn(Vector3 _spawnPos, Vector3 _spawnRot)
        {
            SpawnPos = _spawnPos;
            SpawnRot = _spawnPos;

            Game.PlayerPed.Position = _spawnPos;
            Game.PlayerPed.Rotation = _spawnRot;

            TriggerServerEvent("gamemodes:sv_cl_gotspawn");
        }
    }
}
