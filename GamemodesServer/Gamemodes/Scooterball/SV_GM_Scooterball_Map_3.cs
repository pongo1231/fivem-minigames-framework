using CitizenFX.Core;
using System;

namespace GamemodesServer.Gamemodes.Scooterball
{
    /// <summary>
    /// Scooterball Map 3
    /// </summary>
    public class Scooterball_Map_3 : Scooterball_Map
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Scooterball_Map_3()
        {
            MapFileName = "scooterball/soccer_map_3.xml";

            TimecycMod = "MP_Arena_theme_atlantis";

            Time = new TimeSpan(12, 0, 0);
            Weather = "EXTRASUNNY";

            BallSpawnPos = new Vector3(-279f, -735f, 288f);

            FallOffHeight = 200f;

            RedGoalPos1 = new Vector3(-134f, -719f, 279f);
            RedGoalPos2 = new Vector3(-131f, -735f, 271f);

            BlueGoalPos1 = new Vector3(-428f, -735f, 379f);
            BlueGoalPos2 = new Vector3(-432f, -719f, 271f);
        }
    }
}
