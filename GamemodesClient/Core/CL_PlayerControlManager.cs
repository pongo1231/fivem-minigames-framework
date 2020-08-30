using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Player control manager class
    /// </summary>
    public class PlayerControlManager : BaseScript
    {
        /// <summary>
        /// Whether player has control or not
        /// </summary>
        public static bool HasControl = true;

        /// <summary>
        /// Tick function
        /// </summary>
        [Tick]
        private async Task OnTick()
        {
            // Set player control
            API.SetPlayerControl(Game.Player.Handle, HasControl, 1 << 8);

            await Task.FromResult(0);
        }
    }
}
