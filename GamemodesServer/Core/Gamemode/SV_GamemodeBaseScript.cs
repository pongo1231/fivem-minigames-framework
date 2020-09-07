using GamemodesShared;
using System;
using System.Threading.Tasks;

namespace GamemodesServer.Core.Gamemode
{
    /// <summary>
    /// Gamemode base script class
    /// </summary>
    public abstract class GamemodeBaseScript : GmScript
    {
        /// <summary>
        /// Attribute for calling gamemode function on prestart
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        protected class GamemodePreStartAttribute : Attribute
        {

        }

        /// <summary>
        /// Attribute for calling gamemode function on start
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        protected class GamemodeStartAttribute : Attribute
        {

        }

        /// <summary>
        /// Attribute for calling gamemode function on prestop
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        protected class GamemodePreStopAttribute : Attribute
        {

        }

        /// <summary>
        /// Attribute for calling gamemode function on stop
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        protected class GamemodeStopAttribute : Attribute
        {

        }

        /// <summary>
        /// Attribute for calling gamemode function when timer is up
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        protected class GamemodeTimerUpAttribute : Attribute
        {

        }

        /// <summary>
        /// Attribute for registering event while gamemode is running
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        protected class GamemodeEventHandlerAttribute : Attribute
        {
            public string EventName { get; private set; }

            public GamemodeEventHandlerAttribute(string _eventName)
            {
                EventName = _eventName;
            }
        }

        /// <summary>
        /// Attribute for registering tick function while gamemode is running
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        protected class GamemodeTickAttribute : Attribute
        {

        }

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
        /// Whether this gamemode can be chosen by gamemode manager randomly
        /// </summary>
        public bool ExcludeFromChoicesList { get; set; } = false;

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
