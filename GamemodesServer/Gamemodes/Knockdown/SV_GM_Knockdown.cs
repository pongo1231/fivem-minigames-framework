using CitizenFX.Core;
using GamemodesServer.Core;
using GamemodesServer.Core.Gamemode;
using GamemodesShared;
using System.Threading.Tasks;

namespace GamemodesServer.Gamemodes.Knockdown
{
    /// <summary>
    /// Knockdown gamemode class
    /// </summary>
    public class Knockdown : GamemodeScript<Knockdown_Map>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Knockdown()
        {
            Name = "Knockdown";
            Description = "Don't get pushed off!";
            EventName = "knockdown";
            TimerSeconds = 120;
        }

        /// <summary>
        /// Pre start function
        /// </summary>
        [GamemodePreStart]
        private async Task OnPreStart()
        {
            // Enable scooters
            PlayerScooterManager.Enable("panto", CurrentMap.FallOffHeight);

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
        private async Task OnTimerUp()
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
        /// Player fell off event
        /// </summary>
        /// <param name="_player">Player</param>
        [GamemodeEventHandler("gamemodes:sv_cl_knockdown_felloff")]
        private void OnClientFellOff([FromSource]Player _player)
        {
            // Add score to enemy team
            ScoreManager.AddScore(TeamManager.GetEnemyTeam(_player.GetTeam()));

            // Stop if in overtime
            if (TimerManager.InOvertime)
            {
                StopGamemode();
            }
        }
    }
}