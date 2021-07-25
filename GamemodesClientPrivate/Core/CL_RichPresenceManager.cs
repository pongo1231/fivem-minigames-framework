using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesClient.Core;
using System.Threading.Tasks;

namespace GamemodesClientPrivate.Core
{
    /// <summary>
    /// Rich Presence Manager
    /// </summary>
    public class RichPresenceManager : GmScript
    {
        /// <summary>
        /// Tick function
        /// </summary>
        [Tick]
        private async Task OnTick()
        {
            /* TODO: Well make it actually reflect the current status */

            API.SetDiscordAppId("867553803410341898");

            API.SetDiscordRichPresenceAsset("snail");
            API.SetDiscordRichPresenceAssetText("Happy Snail");

            API.SetDiscordRichPresenceAssetSmall("rank-100");
            API.SetDiscordRichPresenceAssetSmallText("Rank 100");

            API.SetRichPresence("Knockdown");

            API.SetDiscordRichPresenceAction(0, "Learn More", "https://gopong.dev/gamemodes.html");

            await Delay(10000);
        }
    }
}
