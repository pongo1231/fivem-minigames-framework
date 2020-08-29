using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;

namespace GamemodesClient.Core
{
    public class PlayerControlManager : BaseScript
    {
        public static bool HasControl = true;

        [Tick]
        private async Task OnTick()
        {
            API.SetPlayerControl(Game.Player.Handle, HasControl, 1 << 8);

            await Task.FromResult(0);
        }
    }
}
