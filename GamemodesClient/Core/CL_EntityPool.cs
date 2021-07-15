using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Holder of entity pool functions
    /// </summary>
    public static class EntityPool
    {
        /// <summary>
        /// List of spawned in (non-networked) entities
        /// </summary>
        private static List<Entity> s_entities = new List<Entity>();

        /// <summary>
        /// Create and add a new non-networked prop to pool
        /// </summary>
        /// <param name="_model">Model of prop</param>
        /// <param name="_pos">Position of prop</param>
        /// <param name="_dynamic">Whether prop is dynamic</param>
        /// <returns>The new prop</returns>
        public static async Task<Prop> CreateProp(Model _model, Vector3 _pos, bool _dynamic)
        {
            // Request and wait for model to load
            _model.Request();
            while (!_model.IsLoaded)
            {
                await BaseScript.Delay(0);
            }

            // Create new prop
            Prop prop = new Prop(API.CreateObject(_model.Hash, _pos.X, _pos.Y, _pos.Z, false, true, _dynamic));

            // Set position of prop to target position without any offsets
            prop.PositionNoOffset = _pos;

            // Add new prop to pool
            s_entities.Add(prop);

            // Return prop
            return prop;
        }

        /// <summary>
        /// Create and add new non-networked vehicle to pool
        /// </summary>
        /// <param name="_model">Model of vehicle</param>
        /// <param name="_pos">Position of vehicle</param>
        /// <param name="_heading">Heading of vehicle</param>
        /// <returns></returns>
        public static async Task<Vehicle> CreateVehicle(Model _model, Vector3 _pos, float _heading)
        {
            // Request and wait for model to load
            _model.Request();
            while (!_model.IsLoaded)
            {
                await BaseScript.Delay(0);
            }

            // Create a new vehicle
            Vehicle vehicle = new Vehicle(API.CreateVehicle((uint)_model.Hash, _pos.X, _pos.Y, _pos.Z, _heading, false, true));

            // Set position of vehicle to target position without any offsets
            vehicle.PositionNoOffset = _pos;

            // Add new vehicle to pool
            s_entities.Add(vehicle);

            // Return vehicle
            return vehicle;
        }

        /// <summary>
        /// Add an entity to pool
        /// </summary>
        /// <param name="_entity">Entity to add to pool</param>
        public static void AddToPool(Entity _entity)
        {
            // Add entity to pool
            s_entities.Add(_entity);
        }

        /// <summary>
        /// Clear pool of entities
        /// </summary>
        public static void ClearEntities()
        {
            // Remove all entities in pool
            foreach (Entity entity in s_entities)
            {
                entity.Delete();
            }

            // Clear pool
            s_entities.Clear();
        }
    }
}
