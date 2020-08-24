using CitizenFX.Core;
using CitizenFX.Core.UI;
using System.Threading.Tasks;

namespace GamemodesClient.Utils
{
    public static class ScreenUtils
    {
        public static async Task FadeIn()
        {
            Screen.Fading.FadeIn(200);
            await BaseScript.Delay(400);
        }

        public static async Task FadeOut()
        {
            Screen.Fading.FadeOut(200);
            await BaseScript.Delay(200);
        }
    }
}
