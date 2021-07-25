using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesShared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamemodesClient.Core
{
    /// <summary>
    /// Manager for overhead texts of every player
    /// </summary>
    public class PlayerOverheadTextManager : GmScript
    {
        /// <summary>
        /// Whether player names should be visible
        /// </summary>
        public static bool ShowOverheadText = true;

        /// <summary>
        /// Overhead player class
        /// </summary>
        private class OverheadPlayer
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="_player">Player</param>
            public OverheadPlayer(Player _player)
            {
                Player = _player;
            }

            /// <summary>
            /// Player
            /// </summary>
            public Player Player { get; private set; }

            /// <summary>
            /// Handle of player name text
            /// </summary>
            public int OverheadHandle;

            /// <summary>
            /// Whether this player still exists
            /// </summary>
            public bool StillExists = true;
        }

        /// <summary>
        /// List of overhead players
        /// </summary>
        private List<OverheadPlayer> m_overheadPlayers = new List<OverheadPlayer>();

        /// <summary>
        /// Tick function
        /// </summary>
        [Tick]
        private async Task OnTick()
        {
            // Mark all overhead players as non-existant at first
            foreach (var overheadPlayer in m_overheadPlayers)
            {
                overheadPlayer.StillExists = false;
            }

            // Iterate through all players
            int count = 3;
            foreach (var player in Players.Where(_player => _player != Game.Player))
            {
                // Get overhead player from player
                var overheadPlayer = m_overheadPlayers
                    .Find(_overheadPlayer => _overheadPlayer.Player == player);

                if (overheadPlayer == null)
                {
                    // Create new overhead player if it doesn't exist yet
                    overheadPlayer = new OverheadPlayer(player);

                    // Add overhead player to list
                    m_overheadPlayers.Add(overheadPlayer);
                }
                else
                {
                    // Flag overhead player as still existant
                    overheadPlayer.StillExists = true;

                    // Create a new overhead text if one doesn't exist yet
                    if (!API.IsValidMpGamerTagMovie(overheadPlayer.OverheadHandle))
                    {
                        overheadPlayer.OverheadHandle =
                            API.CreateFakeMpGamerTag(player.Character.Handle, player.Name, false,
                            false, null, 0);
                    }

                    // Set visibility status of player name
                    API.SetMpGamerTagVisibility(overheadPlayer.OverheadHandle, 0, ShowOverheadText);

                    // Get team type
                    var teamType = player.GetTeam();

                    if (teamType == ETeamType.TEAM_RED)
                    {
                        // Red color for red team
                        API.SetMpGamerTagColour(overheadPlayer.OverheadHandle, 0, 6);
                    }
                    else if (teamType == ETeamType.TEAM_BLUE)
                    {
                        // Blue color for blue team
                        API.SetMpGamerTagColour(overheadPlayer.OverheadHandle, 0, 202);
                    }
                    else if (teamType == ETeamType.TEAM_UNK)
                    {
                        // White color for unknown team
                        API.SetMpGamerTagColour(overheadPlayer.OverheadHandle, 0, 0);
                    }
                }

                // Amortization
                if (--count == 0)
                {
                    count = 3;

                    await Delay(0);
                }
            }

            // Iterate through all overhead players which are still marked as non existant
            foreach (var overheadPlayer in m_overheadPlayers
                .Where(_overheadPlayer => !_overheadPlayer.StillExists).ToArray())
            {
                // Remove them from list
                m_overheadPlayers.Remove(overheadPlayer);
            }

            await Delay(200);
        }
    }
}