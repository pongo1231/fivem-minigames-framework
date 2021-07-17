using CitizenFX.Core;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace GamemodesClient.Core.Gamemode
{
    /// <summary>
    /// Base Gamemode Script
    /// </summary>
    public abstract class GamemodeScript : GmScript
    {
        /// <summary>
        /// Attribute for calling gamemode function before prestart cam
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        protected class GamemodePreStartAttribute : Attribute
        {

        }

        /// <summary>
        /// Attribute for calling gamemode function after prestart cam
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        protected class GamemodeStartAttribute : Attribute
        {

        }

        /// <summary>
        /// Attribute for calling gamemode function before prestop (winner) cam
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        protected class GamemodePreStopAttribute : Attribute
        {

        }

        /// <summary>
        /// Attribute for calling gamemode function after prestop (winner) cam
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        protected class GamemodeStopAttribute : Attribute
        {

        }

        /// <summary>
        /// Attribute for registering tick function while gamemode is running
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        protected class GamemodeTickAttribute : Attribute
        {

        }

        /// <summary>
        /// Returns whether prestart has been and start hasn't been run yet
        /// </summary>
        protected bool IsGamemodePreStartRunning { get; private set; } = false;

        /// <summary>
        /// Help text to display on every tick
        /// </summary>
        private string m_helpText;

        /// <summary>
        /// Custom prestart function
        /// </summary>
        private Func<Task> m_onPreStart;

        /// <summary>
        /// Custom onstart function
        /// </summary>
        private Func<Task> m_onStart;

        /// <summary>
        /// Custom prestop function
        /// </summary>
        private Func<Task> m_onPreStop;

        /// <summary>
        /// Custom stop function
        /// </summary>
        private Func<Task> m_onStop;

        /// <summary>
        /// List of custom tick functions
        /// </summary>
        private List<Func<Task>> m_onTickFuncs = new List<Func<Task>>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_eventName">Event name to use for event communication between client and server</param>
        /// <param name="_helpText">Help text to display every tick</param>
        public GamemodeScript(string _eventName, string _helpText)
        {
            // Set help text
            m_helpText = _helpText;

            // Register base prestart function
            EventHandlers[$"gamemodes:cl_sv_{_eventName}_prestart"] += new Action(OnPreStart);

            // Register base start function
            EventHandlers[$"gamemodes:cl_sv_{_eventName}_start"] += new Action(OnStart);

            // Register base prestop function
            EventHandlers[$"gamemodes:cl_sv_{_eventName}_prestop"] += new Action(OnPreStop);

            // Register base stop function
            EventHandlers[$"gamemodes:cl_sv_{_eventName}_stop"] += new Action(OnStop);

            // Add help text display tick function to list
            m_onTickFuncs.Add(OnTickHelpText);

            // Helper function for creating a delegate out of method info
            Func<MethodInfo, Func<Task>> createDelegate = (_methodInfo) =>
            {
                return _methodInfo.IsStatic
                    ? (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), _methodInfo)
                    : (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), this, _methodInfo);
            };

            // Go through all functions of child (and all inherited) class(es) via reflection
            foreach (MethodInfo methodInfo in GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
            {
                /* Check for custom attributes and register methods containing them correspondely */

                if (methodInfo.GetCustomAttribute(typeof(GamemodePreStartAttribute)) != null)
                {
                    Debug.WriteLine($"Registering custom OnPreStart for gamemode {methodInfo.DeclaringType.Name}");

                    m_onPreStart = createDelegate(methodInfo);
                }
                else if (methodInfo.GetCustomAttribute(typeof(GamemodeStartAttribute)) != null)
                {
                    Debug.WriteLine($"Registering custom OnStart for gamemode {methodInfo.DeclaringType.Name}");

                    m_onStart = createDelegate(methodInfo);
                }
                else if (methodInfo.GetCustomAttribute(typeof(GamemodePreStopAttribute)) != null)
                {
                    Debug.WriteLine($"Registering custom OnPreStop for gamemode {methodInfo.DeclaringType.Name}");

                    m_onPreStop = createDelegate(methodInfo);
                }
                else if (methodInfo.GetCustomAttribute(typeof(GamemodeStopAttribute)) != null)
                {
                    Debug.WriteLine($"Registering custom OnStop for gamemode {methodInfo.DeclaringType.Name}");

                    m_onStop = createDelegate(methodInfo);
                }
                else if (methodInfo.GetCustomAttribute(typeof(GamemodeTickAttribute)) != null)
                {
                    Debug.WriteLine($"Registering OnTick for gamemode {methodInfo.DeclaringType.Name}");

                    // Add to list of tick delegates
                    m_onTickFuncs.Add(createDelegate(methodInfo));
                }
            }
        }

        /// <summary>
        /// Base prestart function
        /// </summary>
        private async void OnPreStart()
        {
            // Set prestart as running
            IsGamemodePreStartRunning = true;

            // Disable player control during prestart cam
            PlayerControlManager.HasControl = false;

            // Don't draw player names
            PlayerOverheadTextManager.ShowOverheadText = false;

            // Run custom prestart function if available
            if (m_onPreStart != null)
            {
                await m_onPreStart();
            }

            // Register all custom tick functions
            foreach (Func<Task> onTickFunc in m_onTickFuncs)
            {
                Tick += onTickFunc;
            }

            // Respond to server waiting for us
            TriggerServerEvent("gamemodes:sv_cl_prestartedgamemode");
        }

        /// <summary>
        /// Base start function
        /// </summary>
        private async void OnStart()
        {
            // Set prestart as not running anymore
            IsGamemodePreStartRunning = false;

            // Give player back controls after prestart cam
            PlayerControlManager.HasControl = true;

            // Draw player names again
            PlayerOverheadTextManager.ShowOverheadText = true;

            // Play some random music event
            MusicManager.Play();

            // Start drawing scores
            ScoreManager.DrawScores = true;

            // Start oneshot screen effect indicating player can move again
            Screen.Effects.Start(ScreenEffect.MpCelebWinOut);

            // Run custom start function if available
            if (m_onStart != null)
            {
                await m_onStart();
            }

            // Respond to server waiting for us
            TriggerServerEvent("gamemodes:sv_cl_startedgamemode");
        }

        /// <summary>
        /// Base prestop function
        /// </summary>
        private async void OnPreStop()
        {
            // Disable playernames
            PlayerOverheadTextManager.ShowOverheadText = false;

            // Stop music event
            MusicManager.Stop();

            // Stop drawing scores
            ScoreManager.DrawScores = false;

            // Disable boost
            BoostManager.BoostEnabled = false;

            // Unregister all custom tick functions
            foreach (Func<Task> onTickFunc in m_onTickFuncs)
            {
                Tick -= onTickFunc;
            }

            // Run custom prestop function if available
            if (m_onPreStop != null)
            {
                await m_onPreStop();
            }

            // Respond to server waiting for us
            TriggerServerEvent("gamemodes:sv_cl_prestoppedgamemode");
        }

        /// <summary>
        /// Base stop function
        /// </summary>
        private async void OnStop()
        {
            // Cleanup timecyc mod
            TimecycModManager.ClearTimecycMods();

            // Cleanup scooter
            PlayerScooterManager.Cleanup();

            // Run custom stop function if available
            if (m_onStop != null)
            {
                await m_onStop();
            }

            // Respond to server waiting for us
            TriggerServerEvent("gamemodes:sv_cl_stoppedgamemode");
        }

        /// <summary>
        /// Help Text draw tick function
        /// </summary>
        private async Task OnTickHelpText()
        {
            // Draw help text if available and not in prestart cam
            if (!IsGamemodePreStartRunning && m_helpText != null)
            {
                Screen.DisplayHelpTextThisFrame(m_helpText);
            }

            await Task.FromResult(0);
        }
    }
}
