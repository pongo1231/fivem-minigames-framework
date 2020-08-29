using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesShared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamemodesClient.Core
{
    public class PlayerOverheadTextManager : BaseScript
    {
        public static bool ShowOverheadText = true;

        private class OverheadPlayer
        {
            public OverheadPlayer(Player _player)
            {
                Player = _player;
            }

            public Player Player { get; private set; }
            public int OverheadHandle;
            public bool StillExists = true;
        }

        private List<OverheadPlayer> m_overheadPlayers = new List<OverheadPlayer>();

        [Tick]
        private async Task OnTick()
        {
            foreach (OverheadPlayer overheadPlayer in m_overheadPlayers)
            {
                overheadPlayer.StillExists = false;
            }

            foreach (Player player in Players)
            {
                OverheadPlayer overheadPlayer = m_overheadPlayers.Find(_overheadPlayer => _overheadPlayer.Player == player);

                if (overheadPlayer == null)
                {
                    overheadPlayer = new OverheadPlayer(player);

                    m_overheadPlayers.Add(overheadPlayer);
                }
                else
                {
                    overheadPlayer.StillExists = true;

                    if (!API.IsValidMpGamerTagMovie(overheadPlayer.OverheadHandle))
                    {
                        overheadPlayer.OverheadHandle = API.CreateFakeMpGamerTag(player.Character.Handle, player.Name, false, false, null, 0);
                    }

                    API.SetMpGamerTagVisibility(overheadPlayer.OverheadHandle, 0, ShowOverheadText);

                    SHTeamPlayer teamPlayer = TeamManager.TeamPlayers.Find(_teamPlayer => _teamPlayer.PlayerNetId == player.ServerId);

                    if (teamPlayer != null)
                    {
                        ETeamType teamType = (ETeamType)teamPlayer.PlayerTeamType;

                        if (teamType == ETeamType.TEAM_RED)
                        {
                            API.SetMpGamerTagColour(overheadPlayer.OverheadHandle, 0, 6);
                        }
                        else if (teamType == ETeamType.TEAM_BLUE)
                        {
                            API.SetMpGamerTagColour(overheadPlayer.OverheadHandle, 0, 202);
                        }
                    }
                }
            }

            foreach (OverheadPlayer overheadPlayer in m_overheadPlayers.ToArray())
            {
                if (!overheadPlayer.StillExists)
                {
                    m_overheadPlayers.Remove(overheadPlayer);
                }
            }

            await Delay(200);
        }
    }
}
