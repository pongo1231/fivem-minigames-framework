using CitizenFX.Core;

namespace GamemodesServer.Utils
{
    /// <summary>
    /// Holder of Log related functions
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Write line to log
        /// </summary>
        /// <param name="_message">Message</param>
        public static void WriteLine(string _message)
        {
            Debug.WriteLine(_message);
        }
    }
}
