using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesServer.Core.Guards;
using GamemodesServer.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamemodesServer.Core
{
    /// <summary>
    /// Entity pool class
    /// </summary>
    public static class EntityPool
    {
        /// <summary>
        /// List of entities
        /// </summary>
        private static List<Entity> s_entities = new List<Entity>();

        /// <summary>
        /// Create a vehicle
        /// </summary>
        /// <param name="_model">Model of vehicle</param>
        /// <param name="_pos">Position of vehicle</param>
        /// <param name="_rot">Rotation of vehicle</param>
        /// <returns>Created vehicle</returns>
        public static async Task<Vehicle> CreateVehicle(string _model, Vector3 _pos, Vector3 _rot)
        {
            // Lower the entity guard
            EntityGuard.AllowThrough = true;

            // Create vehicle
            Vehicle vehicle = new Vehicle(API.CreateVehicle((uint)API.GetHashKey(_model), _pos.X, _pos.Y, _pos.Z, 0f, true, true));

            // Set vehicle rotation
            vehicle.Rotation = _rot;

            // Add vehicle to pool
            s_entities.Add(vehicle);

            // Wait an extra bit for clients to load this entity
            await BaseScript.Delay(2000);

            // Raise the entity guard again
            EntityGuard.AllowThrough = false;

            // Return vehicle
            return vehicle;
        }

        /// <summary>
        /// Create a prop
        /// </summary>
        /// <param name="_model">Model of prop</param>
        /// <param name="_pos">Position of prop</param>
        /// <param name="_rot">Rotation of prop</param>
        /// <param name="_dynamic">Whether this prop should be dynamic</param>
        /// <returns>Created prop</returns>
        public static async Task<Prop> CreateProp(string _model, Vector3 _pos, Vector3 _rot, bool _dynamic)
        {
            // Lower the entity guard
            EntityGuard.AllowThrough = true;

            // Create prop
            Prop prop = new Prop(API.CreateObjectNoOffset((uint)API.GetHashKey(_model), _pos.X, _pos.Y, _pos.Z, true, true, _dynamic));

            // Set prop rotation
            prop.Rotation = _rot;

            // Add prop to pool
            s_entities.Add(prop);

            // Wait an extra bit for clients to load this entity
            await BaseScript.Delay(2000);

            // Raise the entity guard again
            EntityGuard.AllowThrough = false;

            // Return prop
            return prop;
        }

        /// <summary>
        /// Clear entity pool
        /// </summary>
        public static void ClearEntities()
        {
            // Clear all entities
            foreach (Entity entity in s_entities)
            {
                if (entity.Exists())
                {
                    // Delet this
                    entity.Delete();
                }
            }

            // Clear pool
            s_entities.Clear();
        }
    }
}