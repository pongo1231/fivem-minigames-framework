namespace GamemodesShared
{
    public class SHTeamPlayer
    {
        public SHTeamPlayer(int _playerNetId, int _playerTeamType)
        {
            PlayerNetId = _playerNetId;
            PlayerTeamType = _playerTeamType;
        }

        public int PlayerNetId { get; private set; }
        public int PlayerTeamType { get; private set; }
    }
}
