using CitizenFX.Core;
using System;

namespace GamemodesServer.Gamemodes.Scooterball
{
    public class Scooterball_Map_2 : Scooterball_Map
    {
        public Scooterball_Map_2()
        {
            MapFileName = "scooterball/soccer_map_2.xml";

            TimecycMod = "MP_Arena_theme_atlantis";

            Time = new TimeSpan(12, 0, 0);
            Weather = "EXTRASUNNY";

            BallSpawnPos = new Vector3(771f, 2815f, 185f);

            FallOffHeight = 150f;

            RedGoalPos1 = new Vector3(783f, 2754f, 183f);
            RedGoalPos2 = new Vector3(767f, 2752f, 175f);

            BlueGoalPos1 = new Vector3(767f, 2876f, 183f);
            BlueGoalPos2 = new Vector3(783f, 2878f, 175f);
        }
    }
}
