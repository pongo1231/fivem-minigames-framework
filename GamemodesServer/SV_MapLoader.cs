using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesShared;
using Shared;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace GamemodesServer
{
    public class MapLoader : GmScript
    {
        private class PlayerSpawn
        {
            public PlayerSpawn(Vector3 _spawnPos, Vector3 _spawnRot, EPlayerTeamType _spawnTeamType)
            {
                SpawnPos = _spawnPos;
                SpawnRot = _spawnRot;
                SpawnTeamType = _spawnTeamType;
            }

            public Vector3 SpawnPos { get; private set; }
            public Vector3 SpawnRot { get; private set; }
            public EPlayerTeamType SpawnTeamType { get; private set; } = EPlayerTeamType.TEAM_UNK;
            public Player SpawnOccupiedBy;
        }

        private enum EPlacementType
        {
            Prop = 3,
            Spawn = 1
        }

        private static List<GmProp> s_props = new List<GmProp>();
        private static List<PlayerSpawn> s_playerSpawns = new List<PlayerSpawn>();
        private static List<Player> s_mapPlayers = new List<Player>();
        public static bool MapLoaded { get; private set; } = false;

        public MapLoader()
        {

        }

        [PlayerDropped]
        private void OnPlayerDropped(Player _player)
        {
            s_mapPlayers.Remove(_player);

            PlayerSpawn playerSpawn = s_playerSpawns.Find(_playerSpawn => _playerSpawn.SpawnOccupiedBy == _player);
            if (playerSpawn != null)
            {
                playerSpawn.SpawnOccupiedBy = null;
            }
        }

        public static async Task LoadMap(string _mapName)
        {
            MapLoaded = false;

            TriggerEvent("gamemodes:sv_loadmap", _mapName);

            while (!MapLoaded)
            {
                await Delay(0);
            }
        }

        [EventHandler("gamemodes:sv_loadmap")]
        private void OnLoadMap(string _mapName)
        {
            ClearMap();

            string xml = API.LoadResourceFile(API.GetCurrentResourceName(), _mapName);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            XmlElement placements = xmlDoc["SpoonerPlacements"];

            foreach (XmlElement placement in placements.GetElementsByTagName("Placement"))
            {
                EPlacementType placementType = (EPlacementType)int.Parse(placement["Type"].InnerText);

                switch (placementType)
                {
                    case EPlacementType.Prop:
                        string propName = placement["HashName"].InnerText;

                        XmlElement propPosRot = placement["PositionRotation"];
                        float propX = float.Parse(propPosRot["X"].InnerText);
                        float propY = float.Parse(propPosRot["Y"].InnerText);
                        float propZ = float.Parse(propPosRot["Z"].InnerText);

                        float propPitch = float.Parse(propPosRot["Pitch"].InnerText);
                        float propRoll = float.Parse(propPosRot["Roll"].InnerText);
                        float propYaw = float.Parse(propPosRot["Yaw"].InnerText);

                        bool propCollisions = !bool.Parse(placement["IsCollisionProof"].InnerText);

                        s_props.Add(new GmProp(propName, new Vector3(propX, propY, propZ), new Vector3(propPitch, propRoll, propYaw), propCollisions));

                        break;

                    case EPlacementType.Spawn:
                        string relGroupHash = placement["PedProperties"]["RelationshipGroup"].InnerText;
                        EPlayerTeamType teamType = EPlayerTeamType.TEAM_UNK;

                        if (relGroupHash == "0xb4e845e1")
                        {
                            teamType = EPlayerTeamType.TEAM_RED;
                        }
                        else if (relGroupHash == "0x217b058e")
                        {
                            teamType = EPlayerTeamType.TEAM_BLUE;
                        }

                        XmlElement spawnPosRot = placement["PositionRotation"];
                        float spawnX = float.Parse(spawnPosRot["X"].InnerText);
                        float spawnY = float.Parse(spawnPosRot["Y"].InnerText);
                        float spawnZ = float.Parse(spawnPosRot["Z"].InnerText);

                        float spawnPitch = float.Parse(spawnPosRot["Pitch"].InnerText);
                        float spawnRoll = float.Parse(spawnPosRot["Roll"].InnerText);
                        float spawnYaw = float.Parse(spawnPosRot["Yaw"].InnerText);

                        s_playerSpawns.Add(new PlayerSpawn(new Vector3(spawnX, spawnY, spawnZ), new Vector3(spawnPitch, spawnRoll, spawnYaw), teamType));

                        break;
                }
            }
        }

        [Tick]
        private async Task OnTick()
        {
            if (s_props.Count == 0 || s_playerSpawns.Count == 0)
            {
                return;
            }

            foreach (Player player in PlayerLoadStateManager.GetLoadedInPlayers())
            {
                if (!s_mapPlayers.Contains(player))
                {
                    s_mapPlayers.Add(player);

                    PlayerSpawn freeSpawn = s_playerSpawns.Find(playerSpawn =>
                        (playerSpawn.SpawnOccupiedBy == null || playerSpawn.SpawnOccupiedBy == player) && playerSpawn.SpawnTeamType == TeamManager.GetPlayerTeam(player));

                    await PlayerResponseAwaiter.AwaitResponse(player, "gamemodes:cl_sv_spawnmap", "gamemodes:sv_cl_spawnedmap", s_props);

                    if (freeSpawn != null)
                    {
                        freeSpawn.SpawnOccupiedBy = player;

                        await PlayerResponseAwaiter.AwaitResponse(player, "gamemodes:cl_sv_setspawn", "gamemodes:sv_cl_gotspawn", freeSpawn.SpawnPos, freeSpawn.SpawnRot);
                    }
                }
            }

            MapLoaded = true;

            await Task.FromResult(0);
        }

        public static void ClearMap()
        {
            s_props.Clear();
            s_playerSpawns.Clear();
            s_mapPlayers.Clear();

            MapLoaded = false;

            TriggerClientEvent("gamemodes:cl_sv_clearmap");
        }
    }
}
