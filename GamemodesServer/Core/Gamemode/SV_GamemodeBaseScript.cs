using GamemodesShared;
using System.Threading.Tasks;

namespace GamemodesServer.Core.Gamemode
{
    /// <summary>
    /// Gamemode base script class
    /// </summary>
    public abstract class GamemodeBaseScript : GmScript
    {
        /// <summary>
        /// Name of gamemode
        /// </summary>
        public string Name { get; protected set; } = "???";

        /// <summary>
        /// Description of gamemode
        /// </summary>
        public string Description { get; protected set; } = "???";

        /// <summary>
        /// Name used in events for gamemode
        /// </summary>
        public string EventName { get; protected set; } = "null";

        /// <summary>
        /// Timer for gamemode (-2 for none)
        /// </summary>
        public int TimerSeconds { get; protected set; } = -2;

        /// <summary>
        /// Pre start function
        /// </summary>
        public abstract Task PreStart();

        /// <summary>
        /// Start function
        /// </summary>
        public abstract Task Start();

        /// <summary>
        /// Pre stop function
        /// </summary>
        public abstract Task PreStop();

        /// <summary>
        /// Stop function
        /// </summary>
        public abstract Task Stop();

        /// <summary>
        /// Timer up function
        /// </summary>
        public abstract void TimerUp();

        /// <summary>
        /// Get winner team function
        /// </summary>
        /// <returns>Winner team</returns>
        public abstract ETeamType GetWinnerTeam();
    }
}
