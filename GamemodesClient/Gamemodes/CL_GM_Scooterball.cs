using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using GamemodesClient.Utils;
using GamemodesShared;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace GamemodesClient.Gamemodes
{
    public class Scooterball : GamemodeScript
    {
        private bool m_running = false;

        private GmNetEntity<Vehicle> m_scooter;

        private GmNetEntity<Prop> m_ball;

        private static readonly Vector3 s_ballSpawnPos = new Vector3(1498f, 6600f, 370f);

        private static readonly Vector3 s_redGoalPos1 = new Vector3(1444f, 6623f, 355f);
        private static readonly Vector3 s_redGoalPos2 = new Vector3(1440f, 6607f, 363f);

        private static readonly Vector3 s_blueGoalPos1 = new Vector3(1557f, 6595f, 355f);
        private static readonly Vector3 s_blueGoalPos2 = new Vector3(1556f, 6579f, 363f);

        private Text m_goalsText = new Text(null, new PointF(640f, 50f), 1.5f, Color.FromArgb(255, 255, 255), Font.Pricedown, Alignment.Center, true, true);

        public Scooterball()
        {
            Tick += OnTick;
        }

        [EventHandler("gamemodes:cl_sv_scooterball_start")]
        private void OnStart()
        {
            m_running = true;

            m_scooter = default;
            m_ball = default;

            Game.PlayerPed.IsInvincible = true;
            Game.PlayerPed.CanBeKnockedOffBike = false;

            TriggerServerEvent("gamemodes:sv_cl_scooterball_requestscooter", SpawnManager.SpawnPos, SpawnManager.SpawnRot.X);

            MusicManager.Play();

            API.ClearTimecycleModifier();
            API.ClearExtraTimecycleModifier();

            API.SetTimecycleModifier("WeaponUpgrade");
            //API.SetExtraTimecycleModifier("MP_Arena_theme_saccharine");
            API.PushTimecycleModifier();

            TriggerServerEvent("gamemodes:sv_cl_startedgamemode");
        }

        [EventHandler("gamemodes:cl_sv_scooterball_stop")]
        private void OnStop()
        {
            m_running = false;

            Game.PlayerPed.IsInvincible = false;
            Game.PlayerPed.CanBeKnockedOffBike = true;

            m_scooter.Entity?.Delete();

            BoostManager.DisableBoosting();

            MusicManager.Stop();

            API.ClearTimecycleModifier();
            API.ClearExtraTimecycleModifier();
        }

        [EventHandler("gamemodes:cl_sv_scooterball_spawnedscooter")]
        private async void OnServerSpawnScooter(int _netId)
        {
            m_scooter = new GmNetEntity<Vehicle>(_netId, true);
            while (!m_scooter.Exists)
            {
                await Delay(0);
            }

            m_scooter.Entity.RequestControl();

            //m_scooter.Entity.IsInvincible = true;
            m_scooter.Entity.EngineHealth = float.MaxValue;
            m_scooter.Entity.LockStatus = VehicleLockStatus.StickPlayerInside;
            m_scooter.Entity.EnginePowerMultiplier = 5f;
            m_scooter.Entity.EngineTorqueMultiplier = 5f;
            m_scooter.Entity.IsEngineRunning = true;

            if (TeamManager.TeamType == EPlayerTeamType.TEAM_RED)
            {
                m_scooter.Entity.Mods.CustomPrimaryColor = Color.FromArgb(255, 0, 0);
                m_scooter.Entity.Mods.CustomSecondaryColor = Color.FromArgb(255, 0, 0);
            }
            else if (TeamManager.TeamType == EPlayerTeamType.TEAM_BLUE)
            {
                m_scooter.Entity.Mods.CustomPrimaryColor = Color.FromArgb(0, 0, 255);
                m_scooter.Entity.Mods.CustomSecondaryColor = Color.FromArgb(0, 0, 255);
            }

            m_scooter.Entity.FadeIn();

            BoostManager.EnableBoosting(m_scooter);

            _ = ScreenUtils.FadeIn();
        }

        [EventHandler("gamemodes:cl_sv_scooterball_setball")]
        private void OnServerSetBall(int _netId)
        {
            m_ball = new GmNetEntity<Prop>(_netId, true);

            TriggerServerEvent("gamemodes:sv_cl_scooterball_spawnedball");
        }

        [EventHandler("gamemodes:cl_sv_scooterball_updatescores")]
        private void OnUpdateScores(int _blueGoals, int _redGoals)
        {
            m_goalsText.Caption = $"~r~{_redGoals}   ~b~{_blueGoals}";
        }

        [EventHandler("gamemodes:cl_sv_scooterball_goalscored")]
        private void OnGoalScored(int _teamType, Vector3 _scorePos)
        {
            Function.Call(Hash.ADD_EXPLOSION, _scorePos.X, _scorePos.Y, _scorePos.Z, 63, 100f, true, true, 1f, true);

            Screen.ShowNotification((EPlayerTeamType)_teamType == EPlayerTeamType.TEAM_RED ? "~r~Red~w~ scored a goal!" : "~b~Blue~w~ scored a goal!");
        }

        private async Task OnTick()
        {
            if (!m_running)
            {
                return;
            }

            API.NetworkOverrideClockTime(0, 0, 0);
            API.SetWeatherTypeNowPersist("XMAS");

            m_goalsText.Draw();

            if (TeamManager.TeamType == EPlayerTeamType.TEAM_RED)
            {
                Screen.ShowSubtitle("Shoot the ~r~Ball~w~ into the ~b~Blue Goal.");
            }
            else if (TeamManager.TeamType == EPlayerTeamType.TEAM_BLUE)
            {
                Screen.ShowSubtitle("Shoot the ~b~Ball~w~ into the ~r~Red Goal.");
            }

            Game.DisableControlThisFrame(1, Control.VehicleCinCam);

            if (m_scooter.Exists)
            {
                if (m_scooter.Entity.Driver != Game.PlayerPed)
                {
                    Game.PlayerPed.SetIntoVehicle(m_scooter.Entity, VehicleSeat.Driver);
                }

                Game.DisableControlThisFrame(1, Control.VehicleHandbrake);
                if (Game.IsControlJustPressed(1, Control.Jump) && !m_scooter.Entity.IsInAir)
                {
                    Vector3 vel = m_scooter.Entity.Velocity;

                    vel.Z = 7f;

                    m_scooter.Entity.Velocity = vel;
                }

                if (m_scooter.Entity.Position.Z < 340f || m_scooter.Entity.IsDead || Game.IsControlJustPressed(1, Control.VehicleExit))
                {
                    await ScreenUtils.FadeOut();

                    m_scooter.Entity.RequestControl();

                    m_scooter.Entity.Position = SpawnManager.SpawnPos;
                    m_scooter.Entity.Rotation = default;
                    m_scooter.Entity.Heading = SpawnManager.SpawnRot.X;
                    m_scooter.Entity.Velocity = default;
                    m_scooter.Entity.Repair();

                    m_scooter.Entity.FadeIn();

                    await ScreenUtils.FadeIn();
                }
            }

            if (m_ball.Exists)
            {
                Vector3 markerPos = m_ball.Entity.Position + new Vector3(0f, 0f, 5f);
                Color markerColor = TeamManager.TeamType == EPlayerTeamType.TEAM_RED ? Color.FromArgb(255, 0, 0) : Color.FromArgb(0, 0, 255);
                World.DrawMarker(MarkerType.UpsideDownCone, markerPos, default, default, new Vector3(2f, 2f, 2f), markerColor, true);

                if (m_ball.Entity.AttachedBlip == null)
                {
                    Blip blip = m_ball.Entity.AttachBlip();
                    blip.Color = TeamManager.TeamType == EPlayerTeamType.TEAM_RED ? BlipColor.Red : BlipColor.Blue;
                    blip.Name = "Ball";
                    API.ShowHeightOnBlip(blip.Handle, false);
                }

                if (API.NetworkIsHost())
                {
                    if (m_ball.Entity.IsInArea(s_redGoalPos1, s_redGoalPos2))
                    {
                        TriggerServerEvent("gamemodes:sv_cl_scooterball_bluegoal");
                    }
                    else if (m_ball.Entity.IsInArea(s_blueGoalPos1, s_blueGoalPos2))
                    {
                        TriggerServerEvent("gamemodes:sv_cl_scooterball_redgoal");
                    }
                }
            }

            await Task.FromResult(0);
        }

        [EventHandler("gamemodes:sv_cl_scooterball_resetball")]
        private void OnResetBall()
        {
            if (m_ball.Exists)
            {
                m_ball.Entity.RequestControl();

                m_ball.Entity.Position = s_ballSpawnPos;
                m_ball.Entity.Velocity = new Vector3(0f, 0f, -5f);

                m_ball.Entity.FadeIn();
            }
        }
    }
}