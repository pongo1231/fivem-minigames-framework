using System;

namespace GamemodesShared.Utils
{
    /// <summary>
    /// Utils for Random related stuff
    /// </summary>
    public static class RandomUtils
    {
        /// <summary>
        /// Random
        /// </summary>
        private static Random m_random = new Random();

        /// <summary>
        /// Generate random integer between min and max
        /// </summary>
        /// <param name="_min">Min</param>
        /// <param name="_max">Max (exclusive)</param>
        /// <returns>Random integer</returns>
        public static int RandomInt(int _min, int _max)
        {
            return m_random.Next(_min, _max);
        }

        /// <summary>
        /// Generate random float between min and max
        /// </summary>
        /// <param name="_min">Min</param>
        /// <param name="_max">Max</param>
        /// <returns>Random float</returns>
        public static float RandomFloat(float _min, float _max)
        {
            return (float)m_random.NextDouble() * (_max - _min) + _min;
        }
    }
}
