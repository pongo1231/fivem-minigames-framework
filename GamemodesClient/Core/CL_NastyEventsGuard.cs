using CitizenFX.Core;
using System;

namespace GamemodesClient.Core
{
    public class NastyEventsGuard : BaseScript
    {
        private readonly string[] m_nastyEvents =
        {
            "ambulancier:selfRespawn",
            "bank:transfer",
            "esx_ambulancejob:revive",
            "esx-qalle-jail:openJailMenu",
            "esx_jailer:wysylandoo",
            "esx_policejob:getarrested",
            "esx_society:openBossMenu",
            "esx:spawnVehicle",
            "esx_status:set",
            "HCheat:TempDisableDetection",
            "UnJP"
        };

        public NastyEventsGuard()
        {
            foreach (string _eventName in m_nastyEvents)
            {
                EventHandlers[_eventName] += new Action(SendDropMeToServer);
            }
        }

        private void SendDropMeToServer()
        {
            TriggerServerEvent("gamemodes:sv_cl_dropme");
        }
    }
}
