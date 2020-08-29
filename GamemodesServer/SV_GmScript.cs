using CitizenFX.Core;
using GamemodesServer.Utils;
using System;
using System.Linq;
using System.Reflection;

namespace GamemodesServer
{
    [AttributeUsage(AttributeTargets.Method)]
    public class NewPlayerAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PlayerDroppedAttribute : Attribute
    {

    }

    public class GmScript : BaseScript
    {
        public delegate void NewPlayerHandler(Player _player);
        public static NewPlayerHandler NewPlayer;

        public delegate void PlayerDroppedHandler(Player _player);
        public static PlayerDroppedHandler PlayerDropped;

        public GmScript()
        {
            MethodInfo[] methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            foreach (MethodInfo method in methods.Where(method => method.GetCustomAttribute(typeof(NewPlayerAttribute)) != null))
            {
                Log.WriteLine($"Registering NewPlayer handler {method.DeclaringType.Name}.{method.Name}");

                if (method.IsStatic)
                {
                    NewPlayer += (NewPlayerHandler)Delegate.CreateDelegate(typeof(NewPlayerHandler), method);
                }
                else
                {
                    NewPlayer += (NewPlayerHandler)Delegate.CreateDelegate(typeof(NewPlayerHandler), this, method);
                }
            }

            foreach (MethodInfo method in methods.Where(method => method.GetCustomAttribute(typeof(PlayerDroppedAttribute)) != null))
            {
                Log.WriteLine($"Registering PlayerDropped handler {method.DeclaringType.Name}.{method.Name}");

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
