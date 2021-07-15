using CitizenFX.Core;
using GamemodesServer.Utils;
using System;
using System.Linq;
using System.Reflection;

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
        public static NewPlayerHandler NewPlayer;

        /// <summary>
        /// Delegate for player dropped event
        /// </summary>
        /// <param name="_player">Player</param>
        /// <param name="_dropReason">Reason for drop</param>
        public delegate void PlayerDroppedHandler(Player _player, string _dropReason);

        /// <summary>
        /// Player dropped event
        /// </summary>
        public static PlayerDroppedHandler PlayerDropped;

        /// <summary>
        /// Constructor
        /// </summary>
        public GmScript()
        {
            // Store list of all functions of child (and all inherited) class(es) via reflection
            MethodInfo[] methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            // Iterate through those with new player attribute
            foreach (MethodInfo method in methods.Where(method => method.GetCustomAttribute(typeof(NewPlayerAttribute)) != null))
            {
                Log.WriteLine($"Registering NewPlayer handler {method.DeclaringType.Name}.{method.Name}");

                // Register delegate to new player event
                if (method.IsStatic)
                {
                    NewPlayer += (NewPlayerHandler)Delegate.CreateDelegate(typeof(NewPlayerHandler), method);
                }
                else
                {
                    NewPlayer += (NewPlayerHandler)Delegate.CreateDelegate(typeof(NewPlayerHandler), this, method);
                }
            }

            // Iterate through those with player dropped attribute
            foreach (MethodInfo method in methods.Where(method => method.GetCustomAttribute(typeof(PlayerDroppedAttribute)) != null))
            {
                Log.WriteLine($"Registering PlayerDropped handler {method.DeclaringType.Name}.{method.Name}");

                // Register delegate to player dropped event
                if (method.IsStatic)
                {
                    PlayerDropped += (PlayerDroppedHandler)Delegate.CreateDelegate(typeof(PlayerDroppedHandler), method);
                }
                else
                {
                    PlayerDropped += (PlayerDroppedHandler)Delegate.CreateDelegate(typeof(PlayerDroppedHandler), this, method);
                }
            }
        }
    }
}
