using CitizenFX.Core;
using GamemodesServer.Core;
using GamemodesServer.Core.Map;
using System.Threading.Tasks;

namespace GamemodesServer.Gamemodes.Knockdown
{
    public class Knockdown_Map_1 : Knockdown_Map
    {
        public Knockdown_Map_1()
        {
            MapFileName = "knockdown/knockdown_map_1.xml";

            FallOffHeight = 53f;

            ObstacleSpawnPos1_1 = new Vector3(-1717f, -3021f, 160f);
            ObstacleSpawnPos1_2 = new Vector3(-1775f, -2827f, 163f);
            ObstacleSpawnPos1_Velocity = new Vector3(23f, 0f, -25f);

            ObstacleSpawnPos2_1 = new Vector3(-1331f, -2827f, 160f);
            ObstacleSpawnPos2_2 = new Vector3(-1280f, -3022f, 163f);
            ObstacleSpawnPos2_Velocity = new Vector3(-23f, 0f, -25f);
        }

        /// <summary>
        /// Load function
        /// </summary>
        [GamemodeMapLoad]
        private async Task OnLoad()
        {
            // Also create platform players are on on the server side for better ball sync
            await EntityPool.CreateProp("stt_prop_stunt_bblock_huge_05", new Vector3(-1522.16589f, -2924.78613f, 74.1650925f), default, false);
        }
    }
}
