using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesServer.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamemodesServer
{
    public static class EntityPool
    {
        private static List<Entity> s_entities = new List<Entity>();

        public static async Task<Vehicle> CreateVehicle(string _model, Vector3 _pos, Vector3 _rot)
        {
            EntityPoolGuard.AllowThrough = true;

            Vehicle vehicle = new Vehicle(API.CreateVehicle((uint)API.GetHashKey(_model), _pos.X, _pos.Y, _pos.Z, 0f, true, true));
            vehicle.Rotation = _rot;

            s_entities.Add(vehicle);

            await BaseScript.Delay(750);

            while (!vehicle.Exists())
            {
                await BaseScript.Delay(0);
            }

            EntityPoolGuard.AllowThrough = false;

            return vehicle;
        }

        public static async Task<Prop> CreateProp(string _model, Vector3 _pos, Vector3 _rot, bool _dynamic)
        {
            EntityPoolGuard.AllowThrough = true;

            Prop prop = new Prop(API.CreateObjectNoOffset((uint)API.GetHashKey(_model), _pos.X, _pos.Y, _pos.Z, true, true, _dynamic));
            prop.Rotation = _rot;

            s_entities.Add(prop);

            await BaseScript.Delay(750);

            while (!prop.Exists())
            {
                await BaseScript.Delay(0);
            }

            EntityPoolGuard.AllowThrough = false;

            return prop;
        }

        public static void ClearEntities()
        {
            foreach (Entity entity in s_entities)
            {
                if (entity.Exists())
                {
                    entity.Delete();
                }
            }

            s_entities.Clear();
        }
    }
}