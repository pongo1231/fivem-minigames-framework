using CitizenFX.Core;
using GamemodesServer.Core;
using GamemodesServer.Core.Gamemode;
using GamemodesServer.Utils;
using GamemodesShared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamemodesServer.Gamemodes.Scooterball
{
    /// <summary>
    /// Scooterball gamemode class
    /// </summary>
    public class Scooterball : GamemodeScript<Scooterball_Map>
    {
        /// <summary>
        /// Blue score
        /// </summary>
        private int m_blueGoals;

        /// <summary>
        /// Red score
        /// </summary>
        private int m_redGoals;

        /// <summary>
        /// Whether a goal has been scored
        /// </summary>
        private bool m_scoredGoal = false;

        /// <summary>
        /// List of players
        /// </summary>
        private List<Player> m_scooterPlayers = new List<Player>();

        /// <summary>
        /// Ball entity
        /// </summary>
        private Prop m_ball;

        /// <summary>
        /// Constructor
        /// </summary>
        public Scooterball()
        {
            Name = "Bandito Ball";
            Description = "Propel balls towards the enemies' goal!";
            EventName = "scooterball";
            TimerSeconds = 180;
        }

        /// <summary>
        /// Pre start function
        /// </summary>
        [GamemodePreStart]
        public async Task OnPreStart()
        {
            // Reset scores
            m_redGoals = 0;
            m_blueGoals = 0;

            // Reset goal scored state
            m_scoredGoal = false;

            // Clear players
            m_scooterPlayers.Clear();

            // Spawn the ball
            m_ball = await EntityPool.CreateProp("stt_prop_stunt_soccer_lball", CurrentMap.BallSpawnPos, default, true);

            // Set ball as frozen
            m_ball.IsPositionFrozen = true;

            await Task.FromResult(0);
        }

        /// <summary>
        /// Start function
        /// </summary>
        [GamemodeStart]
        public async Task OnStart()
        {
            // Set ball as not frozen
            m_ball.IsPositionFrozen = false;

            // Give the ball movement
            m_ball.Velocity = new Vector3(0f, 0f, -5f);

            await Task.FromResult(0);
        }

        /// <summary>
        /// Timer up function
        /// </summary>
        [GamemodeTimerUp]
        public async Task OnTimerUp()
        {
            // Go overtime if red score equals blue score
            if (m_redGoals == m_blueGoals)
            {
                TimerManager.SetOvertime();
            }
            else
            {
                // Stop gamemode
                StopGamemode();
            }

            await Task.FromResult(0);
        }

        /// <summary>
        /// Get winner team
        /// </summary>
        public override ETeamType GetWinnerTeam()
        {
            return m_redGoals > m_blueGoals ? ETeamType.TEAM_RED : ETeamType.TEAM_BLUE;
        }

        /// <summary>
        /// Request scooter event by client
        /// </summary>
        /// <param name="_player">Player</param>
        /// <param name="_pos">Position</param>
        /// <param name="_rot">Rotation</param>
        [GamemodeEventHandler("gamemodes:sv_cl_scooterball_requestscooter")]
        private async void OnClientRequestScooter([FromSource]Player _player, Vector3 _pos, Vector3 _rot)
        {
            // Don't get event handler unregistered when client sends garbage to server
            try
            {
                // Check whether player is not registered already
                if (!m_scooterPlayers.Contains(_player))
                {
                    // Wait a bit
                    await Delay(4000);

                    // Add player to list
                    m_scooterPlayers.Add(_player);

                    // Set player position to scooter position
                    _player.Character.Position = _pos;

                    // Wait a bit
                    await Delay(4000);

                    // Spawn scooter
                    Vehicle scooter = await EntityPool.CreateVehicle("rcbandito", _pos, _rot);

                    // Make client aware of scooter
                    _player.TriggerEvent("gamemodes:cl_sv_scooterball_spawnedscooter", scooter.NetworkId);
                }
            }
            catch (Exception _e)
            {
                Log.WriteLine($"{_e}");
            }
        }

        /// <summary>
        /// Tick function for sending states
        /// </summary>
        [GamemodeTick]
        private async Task OnTickSendEvents()
        {
            // Send fall off height to all clients
            TriggerClientEvent("gamemodes:cl_sv_scooterball_setfalloffheight", CurrentMap.FallOffHeight);

            // Send scores to all clients
            TriggerClientEvent("gamemodes:cl_sv_scooterball_updatescores", m_blueGoals, m_redGoals);

            // Send ball network id to all clients if existant
            if (m_ball.Exists())
            {
                TriggerClientEvent("gamemodes:cl_sv_scooterball_setball", m_ball.NetworkId);
            }

            await Delay(500);
        }

        /// <summary>
        /// Tick function for handling the ball
        /// </summary>
        [GamemodeTick]
        private async Task OnTickHandleBall()
        {
            // Respawn the ball if it doesn't exist
            if (!m_ball.Exists())
            {
                // Create ball
                m_ball = await EntityPool.CreateProp("stt_prop_stunt_soccer_lball", CurrentMap.BallSpawnPos, default, true);

                // Give the ball movement
                m_ball.Velocity = new Vector3(0f, 0f, -5f);

                return;
            }

            // Check if ball is off map
            if (m_ball.Position.Z < CurrentMap.FallOffHeight)
            {
                // Respawn ball
                await ResetBall();
            }
            else
            {
                // Check if a goal has been scored
                if (m_ball.Position.IsInArea(CurrentMap.BlueGoalPos1, CurrentMap.BlueGoalPos2))
                {
                    await ScoreGoal(ETeamType.TEAM_RED);
                }
                else if (m_ball.Position.IsInArea(CurrentMap.RedGoalPos1, CurrentMap.RedGoalPos2))
                {
                    await ScoreGoal(ETeamType.TEAM_BLUE);
                }
            }

            await Task.FromResult(0);
        }

        /// <summary>
        /// Score a goal for a team
        /// </summary>
        /// <param name="_teamType">Team</param>
        private async Task ScoreGoal(ETeamType _teamType)
        {
            // Check whether a goal hasn't been scored already
            if (!m_scoredGoal)
            {
                // Set goal as scored
                m_scoredGoal = true;

                // Increase scores for team
                if (_teamType == ETeamType.TEAM_RED)
                {
                    m_redGoals++;
                }
                else if (_teamType == ETeamType.TEAM_BLUE)
                {
                    m_blueGoals++;
                }

                // Broadcast goal scored to all clients
                TriggerClientEvent("gamemodes:cl_sv_scooterball_goalscored", (int)_teamType, m_ball.Position);

                // Wait a bit
                await Delay(3000);

                // Stop gamemode if in overtime, otherwise just reset ball
                if (TimerManager.InOvertime)
                {
                    StopGamemode();
                }
                else
                {
                    await ResetBall();
                }
            }
        }

        /// <summary>
        /// Reset the ball
        /// </summary>
        private async Task ResetBall()
        {
            // Set position to ball spawn position
            m_ball.Position = CurrentMap.BallSpawnPos;

            // Give the ball movement
            m_ball.Velocity = new Vector3(0f, 0f, -5f);

            // Broadcast ball as resetted to all clients
            TriggerClientEvent("gamemodes:sv_cl_scooterball_resetball");

            // Wait a bit
            await Delay(1000);

            // Set no goals scored right now
            m_scoredGoal = false;
        }
    }
}