namespace GamemodesServer.Gamemodes.Hoops
{
    /// <summary>
    /// Hoops map 1 class
    /// </summary>
    public class Hoops_Map_1 : Hoops_Map
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Hoops_Map_1()
        {
            // Set file name
            MapFileName = "hoops/hoops_map_1.xml";

            // Set timecycle modifier
            TimecycMod = "pulse";

            // Set weather
            Weather = "CLOUDS";

            // Set falloff height
            FallOffHeight = 343f;

            // List of hoops
            Hoops = new Hoop[] {
                // Loop
                new Hoop(-68f, -814f, 377.5f),

                // Loop (On Top)
                new Hoop(-63f, -820f, 384f, true),

                // Tunnel
                new Hoop(-131f, -847f, 354f),

                // Windmills Jump
                new Hoop(9f, -819f, 360f),

                // Track
                new Hoop(67.7f, -818f, 381.5f),

                // Tilted Track
                new Hoop(27f, -754f, 366f),

                // Double Ramp
                new Hoop(-147f, -847.5f, 400f),

                // Tunnel below Double Ramp (On Top)
                new Hoop(-147f, -847.5f, 373f),

                // Platform next to Double Ramp
                new Hoop(-203.4f, -847.5f, 360.5f),
                
                // Above Platform next to Double Ramp
                new Hoop(-203.4f, -847.5f, 380f, true),

                // Ramp
                new Hoop(5.3f, -892f, 406f),

                // Wallride to below Ramp
                new Hoop(5f, -889f, 364f, true),

                // Speed Tunnel
                new Hoop(-196f, -730.7f, 411f),

                // Speed Tunnel Blind Launch
                new Hoop(-173f, -740f, 376f)
            };
        }
    }
}
