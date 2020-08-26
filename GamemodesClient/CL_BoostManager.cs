using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using GamemodesClient.Utils;
using System;
using System.Threading.Tasks;

namespace GamemodesClient
{
    public class BoostManager : BaseScript
    {
        public static GmNetEntity<Vehicle> BoostVehicle;
        public static bool BoostEnabled = false;

        private float m_boostFuel = 1f;
        private long m_boostFuelLastTimeStamp;
        private bool m_usingBoost = false;
        private bool m_boostPlayedSound = true;

        [Tick]
        private async Task OnTickHandleBoost()
        {
            if (!BoostVehicle.Exists || !BoostEnabled)
            {
                return;
            }

            API.SetAbilityBarVisibilityInMultiplayer(true);

            long curTimeStamp = API.GetGameTimer();
            long boostDelta = curTimeStamp - m_boostFuelLastTimeStamp;
            m_boostFuelLastTimeStamp = curTimeStamp;

            if (Game.IsControlJustPressed(1, Control.VehicleRocketBoost))
            {
                if (m_boostFuel == 1f)
                {
                    m_usingBoost = true;
                }
            }

            API.SetAbilityBarValue(m_boostFuel, 1f);

            if (m_usingBoost)
            {
                BoostVehicle.Entity.ApplyForceRelative(new Vector3(0f, 0.4f, 0f));

                m_boostFuel = Math.Max(m_boostFuel - boostDelta * 0.0006f, 0f);

                //API.PlaySoundFrontend(-1, "FocusIn", "HintCamSounds", true);
                API.PlaySoundFromEntity(-1, "CLOTHES_THROWN", BoostVehicle.Entity.Handle, "RE_DOMESTIC_SOUNDSET", false, 0);

                PtfxUtils.PlayPtfxOnEntity(BoostVehicle.Entity, "scr_rcbarry2", "muz_clown", true, 0.2f, API.GetEntityBoneIndexByName(BoostVehicle.Entity.Handle, "wheel_lr"));

                PtfxUtils.PlayPtfxOnEntity(BoostVehicle.Entity, "scr_rcbarry2", "muz_clown", true, 0.2f, API.GetEntityBoneIndexByName(BoostVehicle.Entity.Handle, "wheel_rr"));

                Screen.Effects.Start(ScreenEffect.RaceTurbo, 1000);

                if (m_boostFuel == 0f)
                {
                    m_usingBoost = false;

                    m_boostPlayedSound = false;
                }
            }
            else
            {
                m_boostFuel = Math.Min(m_boostFuel + boostDelta * 0.0002f, 1f);

                if (m_boostFuel == 1f && !m_boostPlayedSound)
                {
                    m_boostPlayedSound = true;

                    API.PlaySoundFrontend(-1, "GO", "HUD_MINI_GAME_SOUNDSET", false);
                }
            }

            await Task.FromResult(0);
        }
    }
}
