namespace GamemodesShared
{
    /// <summary>
    /// Shared team player class
    /// </summary>
    public class SHTeamPlayer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_playerNetId">Network id of player</param>
        /// <param name="_playerTeamType">Team of player</param>
        public SHTeamPlayer(int _playerNetId, int _playerTeamType)
        {
            PlayerNetId = _playerNetId;
            PlayerTeamType = _playerTeamType;
        }

        /// <summary>
        /// Network id of player
        /// </summary>
        public int PlayerNetId { get; private set; }

        /// <summary>
        /// Team of player
        /// </summary>
        public int PlayerTeamType { get; private set; }
    }
}
