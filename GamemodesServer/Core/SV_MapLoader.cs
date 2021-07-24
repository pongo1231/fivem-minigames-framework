using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesShared;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace GamemodesServer.Core
{
    /// <summary>
    /// Manager for map loading
    /// </summary>
    public class MapLoader : GmScript
    {
        /// <summary>
        /// Player spawn class
        /// </summary>
        private class PlayerSpawn
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="_spawnPos">Position of spawn</param>
            /// <param name="_spawnRot">Rotation of spawn</param>
            /// <param name="_spawnTeamType">Team that corresponds to spawn</param>
            public PlayerSpawn(Vector3 _spawnPos, Vector3 _spawnRot, ETeamType _spawnTeamType)
            {
                SpawnPos = _spawnPos;
                SpawnRot = _spawnRot;
                SpawnTeamType = _spawnTeamType;
            }

            /// <summary>
            /// Spawn position
            /// </summary>
            public Vector3 SpawnPos { get; private set; }

            /// <summary>
            /// Spawn rotation
            /// </summary>
            public Vector3 SpawnRot { get; private set; }

            /// <summary>
            /// Spawn team
            /// </summary>
            public ETeamType SpawnTeamType { get; private set; } = ETeamType.TEAM_UNK;

            /// <summary>
            /// Player who occupied the spawn
            /// </summary>
            public Player SpawnOccupiedBy;
        }

        /// <summary>
        /// Type of placement
        /// </summary>
        private enum EPlacementType
        {
            Prop = 3,
            Spawn = 1
        }

        /// <summary>
        /// List of props
        /// </summary>
        private static List<SHGmProp> s_props = new List<SHGmProp>();

        /// <summary>
        /// List of player spawns
        /// </summary>
        private static List<PlayerSpawn> s_playerSpawns = new List<PlayerSpawn>();

        /// <summary>
        /// List of players
        /// </summary>
        private static List<Player> s_mapPlayers = new List<Player>();

        /// <summary>
        /// Whether a map has been loaded
        /// </summary>
        public static bool MapLoaded { get; private set; } = false;

        /// <summary>
        /// Player dropped function
        /// </summary>
        /// <param name="_player">Player</param>
        /// <param name="_dropReason">Reason for drop</param>
        [PlayerDropped]
        private void OnPlayerDropped(Player _player, string _dropReason)
        {
            // Remove player from list
            s_mapPlayers.Remove(_player);

            // Check if player occupied a spawn
            var playerSpawn = s_playerSpawns
                .Find(_playerSpawn => _playerSpawn.SpawnOccupiedBy == _player);
            if (playerSpawn != null)
            {
                // Mark spawn as free
                playerSpawn.SpawnOccupiedBy = null;
            }
        }

        /// <summary>
        /// Load a map
        /// </summary>
        /// <param name="_mapName">Name of map</param>
        public static async Task LoadMap(string _mapName)
        {
            // Mark map as not loaded
            MapLoaded = false;

            // Send event to load map
            TriggerEvent("gamemodes:sv_loadmap", _mapName);

            // Wait for map to load
            while (!MapLoaded)
            {
                await Delay(0);
            }
        }

        /// <summary>
        /// Load map event by server
        /// </summary>
        /// <param name="_mapName">Name of map</param>
        [EventHandler("gamemodes:sv_loadmap")]
        private void OnLoadMap(string _mapName)
        {
            // Clear current map
            ClearMap();

            // Get xml from map file
            string xml = API.LoadResourceFile(API.GetCurrentResourceName(), _mapName);

            // Create a new xml document
            var xmlDoc = new XmlDocument();

            // Load xml
            xmlDoc.LoadXml(xml);

            // Get all placements element
            var placements = xmlDoc["SpoonerPlacements"];

            // Iterate through all placements
            foreach (XmlElement placement in placements.GetElementsByTagName("Placement"))
            {
                // Get placement type
                var placementType = (EPlacementType)int.Parse(placement["Type"].InnerText);

                switch (placementType)
                {
                    // Prop
                    case EPlacementType.Prop:
                        // Get prop name
                        var propName = placement["HashName"].InnerText;

                        // Get prop position
                        var propPosRot = placement["PositionRotation"];
                        var propX = float.Parse(propPosRot["X"].InnerText);
                        var propY = float.Parse(propPosRot["Y"].InnerText);
                        var propZ = float.Parse(propPosRot["Z"].InnerText);

                        // Get prop rotation
                        var propPitch = float.Parse(propPosRot["Pitch"].InnerText);
                        var propRoll = float.Parse(propPosRot["Roll"].InnerText);
                        var propYaw = float.Parse(propPosRot["Yaw"].InnerText);

                        // Get prop collisions status
                        var propCollisions = !bool.Parse(placement["IsCollisionProof"].InnerText);

                        // Add prop to list
                        s_props.Add(new SHGmProp(propName, new Vector3(propX, propY, propZ),
                            new Vector3(propPitch, propRoll, propYaw), propCollisions));

                        break;

                    // Ped, used for spawns
                    case EPlacementType.Spawn:
                        // Get relationship group
                        var relGroupHash
                            = placement["PedProperties"]["RelationshipGroup"].InnerText;

                        // Store team type
                        var teamType = ETeamType.TEAM_UNK;

                        // ENEMY group: Red spawn
                        // FRIENDLY group: Blue spawn
                        if (relGroupHash == "0xb4e845e1")
                        {
                            teamType = ETeamType.TEAM_RED;
                        }
                        else if (relGroupHash == "0x217b058e")
                        {
                            teamType = ETeamType.TEAM_BLUE;
                        }

                        // Get spawn position
                        var spawnPosRot = placement["PositionRotation"];
                        var spawnX = float.Parse(spawnPosRot["X"].InnerText);
                        var spawnY = float.Parse(spawnPosRot["Y"].InnerText);
                        var spawnZ = float.Parse(spawnPosRot["Z"].InnerText);

                        // Get spawn rotation
                        var spawnPitch = float.Parse(spawnPosRot["Pitch"].InnerText);
                        var spawnRoll = float.Parse(spawnPosRot["Roll"].InnerText);
                        var spawnYaw = float.Parse(spawnPosRot["Yaw"].InnerText);

                        // Save player spawn
                        s_playerSpawns.Add(new PlayerSpawn(new Vector3(spawnX, spawnY, spawnZ),
                            new Vector3(spawnPitch, spawnRoll, spawnYaw), teamType));

                        break;
                }
            }
        }

        /// <summary>
        /// Tick function
        /// </summary>
        [Tick]
        private async Task OnTick()
        {
            // Check if there are any props and any spawns before continuing
            if (s_props.Count == 0 || s_playerSpawns.Count == 0)
            {
                return;
            }

            // Iterate through all loaded in players
            foreach (var player in PlayerEnrollStateManager.GetLoadedInPlayers())
            {
                // Check if player hasn't been made aware of map yet
                if (!s_mapPlayers.Contains(player))
                {
                    // Add player to list
                    s_mapPlayers.Add(player);

                    // Make client aware of map
                    await PlayerResponseAwaiter.AwaitResponse(player,
                        "gamemodes:cl_sv_spawnmap", "gamemodes:sv_cl_spawnedmap", s_props);

                    // Get first free team spawn
                    var freeSpawn = s_playerSpawns
                        .Find(playerSpawn => (playerSpawn.SpawnOccupiedBy == null
                            || playerSpawn.SpawnOccupiedBy == player)
                            && playerSpawn.SpawnTeamType == player.GetTeam());

                    // Mark spawn as occupied and notify player if spawn was found
                    if (freeSpawn != null)
                    {
                        // Mark spawn as occupied by this player
                        freeSpawn.SpawnOccupiedBy = player;

                        // Make client aware of spawn
                        await PlayerResponseAwaiter.AwaitResponse(player,
                            "gamemodes:cl_sv_setspawn", "gamemodes:sv_cl_gotspawn",
                            freeSpawn.SpawnPos, freeSpawn.SpawnRot);
                    }
                }
            }

            // Mark map as loaded
            MapLoaded = true;

            await Task.FromResult(0);
        }

        /// <summary>
        /// Clear the map
        /// </summary>
        public static void ClearMap()
        {
            // Clear all props
            s_props.Clear();

            // Clear all spawns
            s_playerSpawns.Clear();

            // Clear all players
            s_mapPlayers.Clear();

            // Set map as not loaded
            MapLoaded = false;

            // Make clients aware of map being cleared
            TriggerClientEvent("gamemodes:cl_sv_clearmap");
        }
    }
}