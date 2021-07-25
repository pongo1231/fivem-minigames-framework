using CitizenFX.Core;
using GamemodesShared.Utils;
using System;

namespace GamemodesServer.Core
{
    /// <summary>
    /// Attribute for calling function on new player
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class NewPlayerAttribute : Attribute
    {

    }

    /// <summary>
    /// Attribute for calling function when a player is dropped
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PlayerDroppedAttribute : Attribute
    {

    }

    /// <summary>
    /// Base Script
    /// </summary>
    public abstract class GmScript : BaseScript
    {
        /// <summary>
        /// Delegate for new player event
        /// </summary>
        /// <param name="_player">Player</param>
        public delegate void NewPlayerHandler(Player _player);

        /// <summary>
        /// New player event
        /// </summary>
        public static NewPlayerHandler NewPlayer = null;

        /// <summary>
        /// Delegate for player dropped event
        /// </summary>
        /// <param name="_player">Player</param>
        /// <param name="_dropReason">Reason for drop</param>
        public delegate void PlayerDroppedHandler(Player _player, string _dropReason);

        /// <summary>
        /// Player dropped event
        /// </summary>
        public static PlayerDroppedHandler PlayerDropped = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public GmScript()
        {
            /* Register methods with corresponding attributes */

            ReflectionUtils
                .GetAllMethodsWithAttributeForClass<NewPlayerHandler, NewPlayerAttribute>(this,
                    ref NewPlayer);

            ReflectionUtils
                .GetAllMethodsWithAttributeForClass<PlayerDroppedHandler, PlayerDroppedAttribute>(
                    this, ref PlayerDropped);
        }
    }
}