﻿using CitizenFX.Core;
using GamemodesServer.Core;
using GamemodesServer.Core.Gamemode;
using GamemodesServer.Utils;
using GamemodesShared;
using System.Threading.Tasks;

namespace GamemodesServer.Gamemodes.Scooterball
{
    /// <summary>
    /// Scooterball gamemode class
    /// </summary>
    public class Scooterball : GamemodeScript<Scooterball_Map>
    {
        /// <summary>
        /// Whether a goal has been scored
        /// </summary>
        private bool m_scoredGoal = false;

        /// <summary>
        /// Ball entity
        /// </summary>
        private Prop m_ball = null;

        /// <summary>
        /// Previous position of ball for anti tamper detection
        /// </summary>
        private Vector3 m_prevBallPos = Vector3.Zero;

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
            // Reset goal scored state
            m_scoredGoal = false;

            // Enable scooters
            PlayerScooterManager.Enable(CurrentMap.ScooterModel, CurrentMap.FallOffHeight);

            // Spawn the ball
            m_ball = await EntityPool.CreateProp(
                "stt_prop_stunt_soccer_ball", CurrentMap.BallSpawnPos, default, true);

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
        /// Pre stop function
        /// </summary>
        [GamemodePreStop]
        public async Task OnPreStop()
        {
            // Disable scooters
            PlayerScooterManager.Disable();

            await Task.FromResult(0);
        }

        /// <summary>
        /// Timer up function
        /// </summary>
        [GamemodeTimerUp]
        public async Task OnTimerUp()
        {
            // Go overtime if red score equals blue score
            if (ScoreManager.AreScoresEqual())
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
            return ScoreManager.GetWinnerTeam();
        }

        /// <summary>
        /// Tick function for sending states
        /// </summary>
        [GamemodeTick]
        private async Task OnTickSendEvents()
        {
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
                m_ball = await EntityPool.CreateProp(
                    "stt_prop_stunt_soccer_ball", CurrentMap.BallSpawnPos, default, true);

                // Give the ball movement
                m_ball.Velocity = new Vector3(0f, 0f, -5f);

                return;
            }

            // Check if ball is off map
            if (m_ball.Position.Z < CurrentMap.FallOffHeight
                || (CurrentMap.Boundary1 != default && CurrentMap.Boundary2 != default
                    && !m_ball.Position.IsInArea(CurrentMap.Boundary1, CurrentMap.Boundary2)))
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
        /// Tick function for blocking ball position tampering
        /// </summary>
        [GamemodeTick]
        private async Task OnTickAntiBallTamper()
        {
            // Check if ball position has been tampered with by comparing to previous coords
            if (Vector3.Distance(m_ball.Position, m_prevBallPos) > 5f)
            {
                await ResetBall();
            }

            // Set previous coords to current coords
            m_prevBallPos = m_ball.Position;

            await Delay(50);
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

                // Add score to team
                ScoreManager.AddScore(_teamType);

                // Broadcast goal scored to all clients
                TriggerClientEvent("gamemodes:cl_sv_scooterball_goalscored",
                    (int)_teamType, m_ball.Position);

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

            // Reset stored previous position
            m_prevBallPos = m_ball.Position;

            // Wait a bit
            await Delay(1000);

            // Broadcast ball as resetted to all clients
            TriggerClientEvent("gamemodes:sv_cl_scooterball_resetball");

            // Wait a bit
            await Delay(1000);

            // Give the ball movement
            m_ball.Velocity = new Vector3(0f, 0f, -5f);

            // Set no goals scored right now
            m_scoredGoal = false;
        }
    }
}