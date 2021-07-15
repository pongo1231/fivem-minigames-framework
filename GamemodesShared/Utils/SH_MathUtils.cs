namespace GamemodesShared.Utils
{
    /// <summary>
    /// Utils for math related stuff
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// Get distance between 2 coordinates
        /// </summary>
        /// <param name="_pos1">Position 1</param>
        /// <param name="_pos2">Position 2</param>
        /// <returns>Distance between the two</returns>
        public static float GetDistance(dynamic _pos1, dynamic _pos2)
        {
            return (_pos1 - _pos2).Length();
        }
    }
}
