using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;

namespace GamemodesClient
{
    public class HudManager : BaseScript
    {
        public static bool DisableRadar { private get; set; } = false;

        public HudManager()
        {
            Tick += OnTick;
        }

        private async Task OnTick()
        {
            if (DisableRadar)
            {
                API.DisableRadarThisFrame();
            }

            await Task.FromResult(0);
        }
    }
}
