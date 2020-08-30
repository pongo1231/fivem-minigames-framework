using CitizenFX.Core;
using CitizenFX.Core.UI;
using System.Threading.Tasks;

namespace GamemodesClient.Utils
{
    /// <summary>
    /// Screen utils class
    /// </summary>
    public static class ScreenUtils
    {
        /// <summary>
        /// Fade in screen
        /// </summary>
        public static async Task FadeIn()
        {
            // Fade in screen
            Screen.Fading.FadeIn(200);

            // Wait an extra bit
            await BaseScript.Delay(400);
        }

        /// <summary>
        /// Fade out screen
        /// </summary>
        public static async Task FadeOut()
        {
            // Fade out screen
            Screen.Fading.FadeOut(200);

            // Wait an extra bit
            await BaseScript.Delay(200);
        }

        /// <summary>
        /// Show subitle
        /// </summary>
        /// <param name="_text">Text of subtitle</param>
        public static void ShowSubtitle(string _text)
        {
            Screen.ShowSubtitle(_text, 500);
        }
    }
}
