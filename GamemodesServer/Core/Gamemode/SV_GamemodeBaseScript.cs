using GamemodesShared;
using System.Threading.Tasks;

namespace GamemodesServer.Core.Gamemode
{
    public abstract class GamemodeBaseScript : GmScript
    {
        public string Name { get; protected set; } = "???";
        public string Description { get; protected set; } = "???";
        public string EventName { get; protected set; } = "null";
        public int TimerSeconds { get; protected set; } = 180;

        public abstract Task PreStart();

        public abstract Task Start();

        public abstract Task PreStop();

        public abstract Task Stop();

        public abstract void TimerUp();

        public abstract ETeamType GetWinnerTeam();
    }
}
