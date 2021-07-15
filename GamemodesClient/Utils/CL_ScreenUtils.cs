using CitizenFX.Core;
using CitizenFX.Core.UI;
using System.Threading.Tasks;

namespace GamemodesClient.Utils
{
    /// <summary>
    /// Screen utils
    /// </summary>
    public static class ScreenUtils
    {
        /// <summary>
        /// Whether screen is either fading or faded out
        /// </summary>
        public static bool IsFadedOut
        {
            get
            {
                return Screen.Fading.IsFadingOut || Screen.Fading.IsFadedOut;
            }
        }

        /// <summary>
        /// Whether screen is fading or faded in
        /// </summary>
        public static bool IsFadedIn
        {
            get
            {
                return Screen.Fading.IsFadingIn || Screen.Fading.IsFadedIn;
            }
        }

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
