using CitizenFX.Core.Native;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Holder for demo mode state
    /// </summary>
    public class DemoMode : GmScript
    {
        /// <summary>
        /// Whether resource is in demo mode
        /// You should not draw anything in demo mode
        /// </summary>
        public static bool IsInDemoMode { get; private set; } = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public DemoMode()
        {
            IsInDemoMode = API.GetConvarInt("gamemodes_demo_mode", 0) != 0;
        }
    }
}
