using CitizenFX.Core.Native;
using CitizenFX.Core;

namespace GamemodesServer.Core.Guards
{
    /// <summary>
    /// Guard to block unauthorized networked particles
    /// </summary>
    public class PtfxGuard : GmScript
    {
        /// <summary>
        /// Ptfx Event
        /// </summary>
        [EventHandler("ptFxEvent")]
        private void OnPtfxEvent([FromSource] Player _player, string _data)
        {
            // Cancel it, we don't need any networked particles
            API.CancelEvent();
        }
    }
}
