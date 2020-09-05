using CitizenFX.Core;

namespace GamemodesServer.Gamemodes.Hoops
{
    public class Hoops_Map_1 : Hoops_Map
    {
        public Hoops_Map_1()
        {
            MapFileName = "hoops/hoops_map_1.xml";

            FallOffHeight = 343f;

            Hoops = new Hoop[] {
                // Loop
                new Hoop(new Vector3(-68f, -814f, 378f), new Vector3(0f, 0f, 90f)),

                // Tunnel
                new Hoop(new Vector3(-131f, -847f, 354f), new Vector3(0f, 0f, 90f))
            };
        }
    }
}
