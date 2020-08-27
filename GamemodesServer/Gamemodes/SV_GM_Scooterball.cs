using CitizenFX.Core;
using GamemodesServer.Utils;
using GamemodesShared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamemodesServer.Gamemodes
{
    public class Scooterball : GamemodeScript
    {
        private int m_blueGoals;
        private int m_redGoals;
        private bool m_scoredGoal = false;

        private List<Player> m_scooterPlayers = new List<Player>();

        private Prop m_ball;
        private static readonly Vector3 s_ballSpawnPos = new Vector3(1498f, 6600f, 370f);

        private static readonly Vector3 s_redGoalPos1 = new Vector3(1444f, 6623f, 355f);
        private static readonly Vector3 s_redGoalPos2 = new Vector3(1440f, 6607f, 363f);

        private static readonly Vector3 s_blueGoalPos1 = new Vector3(1557f, 6595f, 355f);
        private static readonly Vector3 s_blueGoalPos2 = new Vector3(1556f, 6579f, 363f);

        public Scooterball()
        {
            Name = "Bandito Ball";
            Description = "Propel balls towards the enemies' goal!";
            EventName = "scooterball";
            TimerSeconds = 180;
        }

        [GamemodePreStart]
        public new async Task OnPreStart()
        {
            m_redGoals = 0;
            m_blueGoals = 0;
            m_scoredGoal = false;

            m_scooterPlayers.Clear();

            await MapLoader.LoadMap("soccer_map_4.xml");

            m_ball = await EntityPool.CreateProp("stt_prop_stunt_soccer_lball", s_ballSpawnPos, default, true);
            m_ball.IsPositionFrozen = true;

            await Task.FromResult(0);
        }

        [GamemodeStart]
        public new async Task OnStart()
        {
            m_ball.IsPositionFrozen = false;
            m_ball.Velocity = new Vector3(0f, 0f, -5f);

            await Task.FromResult(0);
        }

        [GamemodeTimerUp]
        public new async Task OnTimerUp()
        {
            if (m_redGoals == m_blueGoals)
            {
                TimerManager.SetOvertime();
            }
            else
            {
                Stop();
            }

            await Task.FromResult(0);
        }

        public override EPlayerTeamType GetWinnerTeam()
        {
            return m_redGoals > m_blueGoals ? EPlayerTeamType.TEAM_RED : EPlayerTeamType.TEAM_BLUE;
        }

        [GamemodeEventHandler("gamemodes:sv_cl_scooterball_requestscooter")]
        private async void OnClientRequestScooter([FromSource]Player _player, Vector3 _pos, Vector3 _rot)
        {
            try
            {
                if (!m_scooterPlayers.Contains(_player))
                {
                    m_scooterPlayers.Add(_player);

                    Vehicle scooter = await EntityPool.CreateVehicle("rcbandito", _pos, _rot);

                    _player.Character.Position = scooter.Position;

                    await Delay(200);

                    _player.TriggerEvent("gamemodes:cl_sv_scooterball_spawnedscooter", scooter.NetworkId);
                }
            }
            catch (Exception _e)
            {
                Debug.WriteLine($"{_e}");
            }
        }

        [GamemodeTick]
        private async Task OnTickSendEvents()
        {
            TriggerClientEvent("gamemodes:cl_sv_scooterball_updatescores", m_blueGoals, m_redGoals);

            TriggerClientEvent("gamemodes:cl_sv_scooterball_setball", m_ball.NetworkId);

            await Delay(500);
        }

        [GamemodeTick]
        private async Task OnTickHandleBall()
        {
            if (m_ball.Position.Z < 340f)
            {
                await ResetBall();
            }
            else
            {
                if (m_ball.Position.IsInArea(s_blueGoalPos1, s_blueGoalPos2))
                {
                    await ScoreGoal(EPlayerTeamType.TEAM_RED);
                }
                else if (m_ball.Position.IsInArea(s_redGoalPos1, s_redGoalPos2))
                {
                    await ScoreGoal(EPlayerTeamType.TEAM_BLUE);
                }
            }

            await Task.FromResult(0);
        }

        private async Task ScoreGoal(EPlayerTeamType _teamType)
        {
            if (!m_scoredGoal)
            {
                m_scoredGoal = true;

                if (_teamType == EPlayerTeamType.TEAM_RED)
                {
                    m_redGoals++;
                }
                else if (_teamType == EPlayerTeamType.TEAM_BLUE)
                {
                    m_blueGoals++;
                }

                TriggerClientEvent("gamemodes:cl_sv_scooterball_goalscored", (int)_teamType, m_ball.Position);

                await Delay(3000);

                if (TimerManager.InOvertime)
                {
                    Stop();
                }
                else
                {
                    await ResetBall();
                }
            }
        }

        private async Task ResetBall()
        {
            m_ball.Position = s_ballSpawnPos;
            m_ball.Velocity = new Vector3(0f, 0f, -5f);

            TriggerClientEvent("gamemodes:sv_cl_scooterball_resetball");

            await Delay(1000);

            m_scoredGoal = false;
        }
    }
}