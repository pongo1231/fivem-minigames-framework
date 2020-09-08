using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Radar manager class
    /// </summary>
    public class RadarManager : GmScript
    {
        /// <summary>
        /// Minimap scaleform
        /// </summary>
        //Scaleform m_minimap = new Scaleform("minimap");

        /// <summary>
        /// Whether this is the first tick
        /// </summary>
        bool m_firstTick = true;

        [Tick]
        private async Task OnTick()
        {
            if (m_firstTick)
            {
                // Not first tick anymore
                m_firstTick = false;

                // Refresh minimap scaleform by forcing big mode
                //API.SetRadarBigmapEnabled(true, false);

                // Wait
                await Delay(0);
            }
            else
            {
                // Disable big mode of minimap
                //API.SetRadarBigmapEnabled(false, false);

                // Force blips only minimap mode
                API.HideMinimapExteriorMapThisFrame();
                API.HideMinimapInteriorMapThisFrame();

                // Disable health and armor bar
                //m_minimap.CallFunction("SETUP_HEALTH_ARMOUR", 3);
            }

            await Task.FromResult(0);
        }
    }
}
