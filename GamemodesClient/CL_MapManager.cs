using CitizenFX.Core;
using System;
using System.Collections.Generic;

namespace GamemodesClient
{
    public class MapManager : BaseScript
    {
        public MapManager()
        {

        }

        [EventHandler("gamemodes:cl_sv_spawnmap")]
        private async void OnSpawnMap(List<dynamic> _props)
        {
            foreach (dynamic prop in _props)
            {
                GmNetEntity<Prop> spawnedProp = await EntityPool.CreateProp(prop.PropName, prop.PropPos, false, false);
                spawnedProp.Entity.Rotation = prop.PropRot;
                spawnedProp.Entity.IsCollisionEnabled = prop.PropCollisions;
                spawnedProp.Entity.IsPositionFrozen = true;
            }

            TriggerServerEvent("gamemodes:sv_cl_spawnedmap");
        }

        [EventHandler("gamemodes:cl_sv_clearmap")]
        private void OnClearMap()
        {
            EntityPool.ClearEntities();
        }

        [EventHandler("gamemodes:cl_sv_deleteentity")]
        private void OnServerDeleteEntity(int _netId)
        {
            new GmNetEntity<Prop>(_netId, true).Entity?.Delete();
        }
    }
}
