using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesServer.Core.Gamemode;
using GamemodesServer.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamemodesServer.Core
{
    /// <summary>
    /// Handler for gamemode stop votes by players (temporary until stable release)
    /// </summary>
    public class StopGamemodeVotingHandler : GmScript
    {
        /// <summary>
        /// List of players to await a vote for
        /// </summary>
        private List<Player> m_votingPlayers = new List<Player>();

        /// <summary>
        /// Whether a vote is currently running
        /// </summary>
        private bool m_voteRunning = false;

        /// <summary>
        /// Time until vote runs out
        /// </summary>
        private long m_voteTimeoutTimestamp;

        /// <summary>
        /// Player dropped function
        /// </summary>
        /// <param name="_player">Player</param>
        /// <param name="_dropReason">Reason for drop</param>
        [PlayerDropped]
        private void OnPlayerDropped(Player _player, string _dropReason)
        {
            // Remove player from list
            m_votingPlayers.Remove(_player);
        }

        /// <summary>
        /// Vote start command function
        /// </summary>
        /// <param name="_player">Player</param>
        [Command("votestop")]
        private void OnCommandVoteStop([FromSource]Player _player)
        {
            // Don't continue if a vote is already running
            if (m_voteRunning)
            {
                _player.SendMessage("^1A stop vote is already running! Vote with /y instead.");

                return;
            }

            // Set vote as running
            m_voteRunning = true;

            // Set vote timeout
            m_voteTimeoutTimestamp = API.GetGameTimer() + 30000;

            // Add all currently loaded in players to list (except voter)
            foreach (Player player in PlayerEnrollStateManager.GetLoadedInPlayers().Where(pplayer => pplayer != _player))
            {
                m_votingPlayers.Add(player);
            }

            // Notify everyone of ongoing vote if there are more players in game than voter
            if (m_votingPlayers.Count > 0)
            {
                ChatUtils.SendMessage($"^3{_player.Name} started a vote for stopping the current gamemode and starting a new one! Respond with /y to vote for it. All current players need to vote for it to succeed.");
            }
        }

        /// <summary>
        /// Vote command function
        /// </summary>
        /// <param name="_player">Player</param>
        [Command("y")]
        private void OnCommandVoteYes([FromSource]Player _player)
        {
            // Don't continue if a vote isn't running
            if (!m_voteRunning)
            {
                _player.SendMessage("^1There is no stop vote running currently. Start a vote with /votestop instead.");

                return;
            }

            // Warn player if they aren't allowed to vote
            if (!m_votingPlayers.Contains(_player))
            {
                _player.SendMessage("^1You either already voted or joined after the vote started!");

                return;
            }

            // Remove voting player
            m_votingPlayers.Remove(_player);

            _player.SendMessage("^2Counted vote!");
        }

        /// <summary>
        /// Tick function
        /// </summary>
        [Tick]
        private async Task OnTick()
        {
            // Check if vote is running
            if (m_voteRunning)
            {
                // Accept vote if no voting players are left
                if (m_votingPlayers.Count == 0)
                {
                    m_voteRunning = false;

                    GamemodeManager.StopGamemode();

                    ChatUtils.SendMessage("^2Gamemode has been stopped by vote!");
                }
                else
                {
                    /* Stop vote if time is up */

                    long curTimestamp = API.GetGameTimer();

                    if (m_voteTimeoutTimestamp < curTimestamp)
                    {
                        m_voteRunning = false;

                        ChatUtils.SendMessage("^1Vote for stopping the gamemode has not succeeded!");
                    }
                }
            }

            await Delay(100);
        }

        /// <summary>
        /// Show gamemode stop vote reminder to everyone
        /// </summary>
        public static void ShowReminder()
        {
            ChatUtils.SendMessage("^2Should the gamemode softlock or cause any other issues you can start a vote to stop it with /votestop.");
        }
    }
}
