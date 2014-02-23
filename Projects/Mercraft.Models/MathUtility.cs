

using System;

namespace Mercraft.Models
{
    public static class MathUtility
    {
        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        public static double Deg2Rad(double degrees)
        {
            return Math.PI * degrees / 180.0;
        }

        /// <summary>
        /// Converts radians to degrees
        /// </summary>
        public static double Rad2Deg(double radians)
        {
            return 180.0 * radians / Math.PI;
        }
    }
}
