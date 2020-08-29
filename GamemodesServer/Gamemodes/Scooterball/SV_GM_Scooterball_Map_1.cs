using CitizenFX.Core;

namespace GamemodesServer.Gamemodes.Scooterball
{
    public class Scooterball_Map_1 : Scooterball_Map
    {
        public Scooterball_Map_1()
        {
            MapFileName = "soccer_map_4.xml";

            BallSpawnPos = new Vector3(1498f, 6600f, 370f);
            
            RedGoalPos1 = new Vector3(1444f, 6623f, 355f);
            RedGoalPos2 = new Vector3(1440f, 6607f, 363f);
            
            BlueGoalPos1 = new Vector3(1557f, 6595f, 355f);
            BlueGoalPos2 = new Vector3(1556f, 6579f, 363f);
        }
    }
}
