using CitizenFX.Core;
using GamemodesShared;
using System.Threading.Tasks;

namespace GamemodesServer.Gamemodes
{
    public class Scooterball : GamemodeScript
    {
        private int m_blueGoals;
        private int m_redGoals;
        private bool m_scoredGoal = false;

        private Prop m_ball;
        private static readonly Vector3 s_ballSpawnPos = new Vector3(1498f, 6600f, 370f);

        public Scooterball() : base("Scooter Ball", "scooterball", 180)
        {
            
        }

        public override async Task OnStart()
        {
            m_redGoals = 0;
            m_blueGoals = 0;

            await MapLoader.LoadMap("soccer_map_3.xml");

            m_ball = await EntityPool.CreateProp("stt_prop_stunt_soccer_lball", s_ballSpawnPos, default, true);
            m_ball.Velocity = new Vector3(0f, 0f, -5f);
        }

        [EventHandler("gamemodes:sv_cl_scooterball_bluegoal")]
        private void OnBlueGoal()
        {
            if (!m_scoredGoal)
            {
                m_blueGoals++;
                m_scoredGoal = true;

                TriggerClientEvent("gamemodes:cl_sv_scooterball_goalscored", (int)EPlayerTeamType.TEAM_BLUE, m_ball.Position);

                if (TimerManager.InOvertime)
                {
                    Stop();
                }
            }
        }

        [EventHandler("gamemodes:sv_cl_scooterball_redgoal")]
        private void OnRedGoal()
        {
            if (!m_scoredGoal)
            {
                m_redGoals++;
                m_scoredGoal = true;

                TriggerClientEvent("gamemodes:cl_sv_scooterball_goalscored", (int)EPlayerTeamType.TEAM_RED, m_ball.Position);

                if (TimerManager.InOvertime)
                {
                    Stop();
                }
            }
        }

        [EventHandler("gamemodes:sv_cl_scooterball_requestscooter")]
        private async void OnClientRequestScooter([FromSource]Player _player, Vector3 _pos, float _heading)
        {
            Vehicle scooter = await EntityPool.CreateVehicle("rcbandito", _pos, _heading);

            _player.Character.Position = scooter.Position;

            await Delay(200);

            _player.TriggerEvent("gamemodes:cl_sv_scooterball_spawnedscooter", scooter.NetworkId);
        }

        [EventHandler("gamemodes:sv_cl_scooterball_resetball")]
        private async void OnClientResetBall()
        {
            await Delay(1000);

            m_scoredGoal = false;
        }

        public override async Task OnTick()
        {
            TriggerClientEvent("gamemodes:cl_sv_scooterball_updatescores", m_blueGoals, m_redGoals);

            TriggerClientEvent("gamemodes:cl_sv_scooterball_setball", m_ball.NetworkId);

            await Delay(500);
        }

        public override void OnTimerUp()
        {
            if (m_redGoals == m_blueGoals)
            {
                TimerManager.SetOvertime();
            }
            else
            {
                Stop();
            }
        }
    }
}
