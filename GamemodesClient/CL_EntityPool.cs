using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesClient.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamemodesClient
{
    public static class EntityPool
    {
        private static List<GmNetEntity<Entity>> s_entities = new List<GmNetEntity<Entity>>();

        public static async Task<GmNetEntity<Prop>> CreateProp(Model _model, Vector3 _pos, bool _dynamic, bool _networked)
        {
            _model.Request();
            while (!_model.IsLoaded)
            {
                await BaseScript.Delay(0);
            }

            GmNetEntity<Prop> prop = new GmNetEntity<Prop>(new Prop(API.CreateObject(_model.Hash, _pos.X, _pos.Y, _pos.Z, _networked, false, _dynamic)));

            if (_networked)
            {
                prop.Entity.RequestControl();

                //API.SetNetworkIdCanMigrate(prop.NetworkId, false);
            }

            prop.Entity.PositionNoOffset = _pos;

            s_entities.Add(new GmNetEntity<Entity>(prop.Entity));

            return prop;
        }

        public static async Task<GmNetEntity<Vehicle>> CreateVehicle(Model _model, Vector3 _pos, float _heading, bool _networked)
        {
            _model.Request();
            while (!_model.IsLoaded)
            {
                await BaseScript.Delay(0);
            }

            GmNetEntity<Vehicle> vehicle = new GmNetEntity<Vehicle>(new Vehicle(API.CreateVehicle((uint)_model.Hash, _pos.X, _pos.Y, _pos.Z, _heading, _networked, false)));

            if (_networked)
            {
                vehicle.Entity.RequestControl();

                //API.SetNetworkIdCanMigrate(prop.NetworkId, false);
            }

            vehicle.Entity.PositionNoOffset = _pos;

            s_entities.Add(new GmNetEntity<Entity>(vehicle.Entity));

            return vehicle;
        }

        public static void AddToPool(Entity _entity)
        {
            s_entities.Add(new GmNetEntity<Entity>(_entity));
        }

        public static void AddToPool(GmNetEntity<Entity> _entity)
        {
            s_entities.Add(_entity);
        }

        public static void AddToPool(GmNetEntity<Ped> _entity)
        {
            s_entities.Add(new GmNetEntity<Entity>(_entity.Entity));
        }

        public static void AddToPool(GmNetEntity<Vehicle> _entity)
        {
            s_entities.Add(new GmNetEntity<Entity>(_entity.Entity));
        }

        public static void AddToPool(GmNetEntity<Prop> _entity)
        {
            s_entities.Add(new GmNetEntity<Entity>(_entity.Entity));
        }

        public static void ClearEntities()
        {
            foreach (GmNetEntity<Entity> entity in s_entities)
            {
                entity.Entity.Delete();
            }

            s_entities.Clear();
        }
    }
}
