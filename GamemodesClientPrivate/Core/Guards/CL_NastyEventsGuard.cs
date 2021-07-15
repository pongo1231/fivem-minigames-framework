using GamemodesClient.Core;
using System;

namespace GamemodesClientPrivate.Core.Guards
{
    /// <summary>
    /// Illegal event manager class
    /// </summary>
    public class NastyEventsGuard : GmScript
    {
        /// <summary>
        /// List of events which are definitely not sent with good intentions
        /// </summary>
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

        /// <summary>
        /// Constructor
        /// </summary>
        public NastyEventsGuard()
        {
            foreach (string _eventName in m_nastyEvents)
            {
                EventHandlers[_eventName] += new Action(SendDropMeToServer);
            }
        }

        /// <summary>
        /// Indicates to server to drop this client
        /// </summary>
        private void SendDropMeToServer()
        {
            TriggerServerEvent("gamemodes:sv_cl_dropme");
        }
    }
}
