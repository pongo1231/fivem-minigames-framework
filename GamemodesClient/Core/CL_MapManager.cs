using CitizenFX.Core;
using System;
using System.Collections.Generic;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Map manager class
    /// </summary>
    public class MapManager : BaseScript
    {
        /// <summary>
        /// Spawn map event from server
        /// </summary>
        /// <param name="_props">List of SHGmProps</param>
        [EventHandler("gamemodes:cl_sv_spawnmap")]
        private async void OnSpawnMap(List<dynamic> _props)
        {
            // Respond to server waiting for us
            TriggerServerEvent("gamemodes:sv_cl_spawnedmap");

            // Iterate through all SHGmProps
            foreach (dynamic prop in _props)
            {
                // Create prop
                Prop spawnedProp = await EntityPool.CreateProp(prop.PropName, prop.PropPos, false);

                // Set rotation
                spawnedProp.Rotation = prop.PropRot;

                // Set collision status
                spawnedProp.IsCollisionEnabled = prop.PropCollisions;

                // Set as frozen
                spawnedProp.IsPositionFrozen = true;
            }
        }

        /// <summary>
        /// Clear map event from server
        /// </summary>
        [EventHandler("gamemodes:cl_sv_clearmap")]
        private void OnClearMap()
        {
            // Clear entity pool
            EntityPool.ClearEntities();
        }
    }
}
