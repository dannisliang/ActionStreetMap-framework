﻿

using System;
using System.Collections.Generic;

namespace Mercraft.Core.Utilities
{
    public static class MathUtility
    {
        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        public static double Deg2Rad(double degrees)
        {
            return Math.PI*degrees/180.0;
        }

        /// <summary>
        /// Converts radians to degrees
        /// </summary>
        public static double Rad2Deg(double radians)
        {
            return 180.0*radians/Math.PI;
        }

        /// <summary>
        /// Compares two float numbers using epsilon
        /// </summary>
        public static bool AreEqual(float a, float b, float epsilon = float.Epsilon)
        {
            return Math.Abs(a - b) < epsilon;
        }

        public static bool AreEqual(double a, double b, double epsilon = double.Epsilon)
        {
            return Math.Abs(a - b) < epsilon;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var random = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
