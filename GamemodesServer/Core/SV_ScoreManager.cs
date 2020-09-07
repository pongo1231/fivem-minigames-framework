using CitizenFX.Core;
using GamemodesShared;
using System.Threading.Tasks;

namespace GamemodesServer.Core
{
    /// <summary>
    /// Scores manager class
    /// </summary>
    public class ScoreManager : GmScript
    {
        /// <summary>
        /// Red score
        /// </summary>
        private static int s_redScore;

        /// <summary>
        /// Blue score
        /// </summary>
        private static int s_blueScore;

        /// <summary>
        /// Tick function
        /// </summary>
        [Tick]
        private async Task OnTick()
        {
            // Send current scores to all clients
            TriggerClientEvent("gamemodes:cl_sv_updatescores", s_redScore, s_blueScore);

            await Delay(200);
        }

        /// <summary>
        /// Add to current score of team
        /// </summary>
        /// <param name="_teamType">Either red or blue team</param>
        /// <param name="_amount">Amount to add to current score</param>
        public static void AddScore(ETeamType _teamType, int _amount = 1)
        {
            switch (_teamType)
            {
                case ETeamType.TEAM_RED:
                    s_redScore += _amount;

                    break;
                case ETeamType.TEAM_BLUE:
                    s_blueScore += _amount;

                    break;
            }
        }

        /// <summary>
        /// Whether red and blue scores are equal
        /// </summary>
        /// <returns>Whether red score equals blue score</returns>
        public static bool AreScoresEqual()
        {
            return s_redScore == s_blueScore;
        }

        /// <summary>
        /// Get the team with the highest score
        /// </summary>
        /// <returns>Team with the highest score (TEAM_UNK if both are equal)</returns>
        public static ETeamType GetWinnerTeam()
        {
            if (s_redScore > s_blueScore)
            {
                // Red won
                return ETeamType.TEAM_RED;
            }
            else if (s_blueScore > s_redScore)
            {
                // Blue won
                return ETeamType.TEAM_BLUE;
            }
            else
            {
                // Neither won
                return ETeamType.TEAM_UNK;
            }
        }

        /// <summary>
        /// Get score of either red or blue team
        /// </summary>
        /// <param name="_teamType">Red or blue team</param>
        /// <returns>Score of either red or blue team, otherwise 0 if invalid team</returns>
        public static int GetScore(ETeamType _teamType)
        {
            switch (_teamType)
            {
                case ETeamType.TEAM_RED:
                    return s_redScore;
                case ETeamType.TEAM_BLUE:
                    return s_blueScore;
            }

            // Invalid team :(
            return 0;
        }

        /// <summary>
        /// Reset all scores to zero
        /// </summary>
        public static void ResetScores()
        {
            s_redScore = 0;
            s_blueScore = 0;
        }
    }
}
