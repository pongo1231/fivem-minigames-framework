using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using GamemodesClient.Utils;
using System;
using System.Threading.Tasks;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Manager for player vehicle boost
    /// </summary>
    public class BoostManager : GmScript
    {
        /// <summary>
        /// Target vehicle to manage boost of
        /// </summary>
        public static GmNetEntity<Vehicle> BoostVehicle;

        /// <summary>
        /// Whether boost should be enabled or not
        /// </summary>
        public static bool BoostEnabled = false;

        /// <summary>
        /// Current fuel level (1 is max)
        /// </summary>
        private float m_boostFuel = 1f;

        /// <summary>
        /// Timestamp to save from last tick execution
        /// </summary>
        private long m_boostFuelLastTimeStamp;

        /// <summary>
        /// Whether boost was triggered
        /// </summary>
        private bool m_usingBoost = false;

        /// <summary>
        /// Whether sound played when boost is fully recharged has been played yet
        /// </summary>
        private bool m_boostRechargedPlayedSound = true;

        /// <summary>
        /// Tick function
        /// </summary>
        /// <returns></returns>
        [Tick]
        private async Task OnTickHandleBoost()
        {
            // Abort if either target vehicle doesn't exist or boost isn't enabled
            if (!BoostVehicle.Exists || !BoostEnabled)
            {
                return;
            }

            // Show ability bar to indicate fuel recharge level
            API.SetAbilityBarVisibilityInMultiplayer(true);

            // Save current timestamp
            var curTimeStamp = API.GetGameTimer();

            // Calculate delta from current and last timestamp
            var boostDelta = curTimeStamp - m_boostFuelLastTimeStamp;

            // Set last timestamp to current timestamp
            m_boostFuelLastTimeStamp = curTimeStamp;

            // Check if boost button has been pressed
            if (Game.IsControlJustPressed(1, Control.VehicleRocketBoost))
            {
                // Trigger boost if fuel is full
                if (m_boostFuel == 1f)
                {
                    m_usingBoost = true;
                }
            }

            // Set ability bar level to current fuel level and set max to 1
            API.SetAbilityBarValue(m_boostFuel, 1f);

            // Check if boost is currently active
            if (m_usingBoost)
            {
                // Give the current vehicle a force push
                BoostVehicle.Entity.ApplyForceRelative(new Vector3(0f, 0.4f, 0f));

                // Calculate new fuel level
                m_boostFuel = Math.Max(m_boostFuel - boostDelta * 0.0006f, 0f);

                // Play active boost sound
                API.PlaySoundFromEntity(-1, "CLOTHES_THROWN", BoostVehicle.Entity.Handle,
                    "RE_DOMESTIC_SOUNDSET", false, 0);

                // Display networked boost particle effects
                BoostVehicle.Entity.PlayPtfxOnEntity("scr_rcbarry2", "muz_clown", true, 0.2f,
                    API.GetEntityBoneIndexByName(BoostVehicle.Entity.Handle, "wheel_lr"));
                BoostVehicle.Entity.PlayPtfxOnEntity("scr_rcbarry2", "muz_clown", true, 0.2f,
                    API.GetEntityBoneIndexByName(BoostVehicle.Entity.Handle, "wheel_rr"));

                // Show boost screen effect
                Screen.Effects.Start(ScreenEffect.RaceTurbo, 1000);

                // Check if fuel is empty
                if (m_boostFuel <= 0f)
                {
                    // Set boost as not active anymore
                    m_usingBoost = false;

                    // Set boost recharged sound as not played yet
                    m_boostRechargedPlayedSound = false;
                }
            }
            else
            {
                // Calculate new fuel level
                m_boostFuel = Math.Min(m_boostFuel + boostDelta * 0.0002f, 1f);

                // Check if fuel is full and sound hasn't been played yet
                if (m_boostFuel >= 1f && !m_boostRechargedPlayedSound)
                {
                    // Set fuel fully recharged sound flag as played
                    m_boostRechargedPlayedSound = true;

                    // Play sound
                    API.PlaySoundFrontend(-1, "GO", "HUD_MINI_GAME_SOUNDSET", false);
                }
            }

            await Task.FromResult(0);
        }
    }
}