using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamemodesServer
{
    public static class EntityPool
    {
        private static List<Entity> s_entities = new List<Entity>();

        public static async Task<Vehicle> CreateVehicle(string _model, Vector3 _pos, float _heading)
        {
            Vehicle vehicle = new Vehicle(API.CreateVehicle((uint)API.GetHashKey(_model), _pos.X, _pos.Y, _pos.Z, _heading, true, true));

            s_entities.Add(vehicle);

            await BaseScript.Delay(750);

            return vehicle;
        }

        public static async Task<Prop> CreateProp(string _model, Vector3 _pos, Vector3 _rot, bool _dynamic)
        {
            Prop prop = new Prop(API.CreateObjectNoOffset((uint)API.GetHashKey(_model), _pos.X, _pos.Y, _pos.Z, true, false, _dynamic));
            prop.Rotation = _rot;

            s_entities.Add(prop);

            await BaseScript.Delay(750);

            return prop;
        }

        public static void ClearEntities()
        {
            foreach (Entity entity in s_entities)
            {
                try
                {
                    BaseScript.TriggerClientEvent("gamemodes:cl_sv_deleteentity", entity.NetworkId);
                }
                catch (System.InvalidOperationException)
                {
                    Debug.WriteLine("Couldn't find and delete entity!");
                }
            }

            s_entities.Clear();
        }
    }
}