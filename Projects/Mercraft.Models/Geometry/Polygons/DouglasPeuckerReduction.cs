using System;
using System.Collections.Generic;
using Mercraft.Core;

namespace Mercraft.Models.Geometry.Polygons
{
    /// <summary>
    ///     Implements the Douglas Peucker algorithim to reduce the number of points
    /// </summary>
    internal class DouglasPeuckerReduction
    {
        private static List<Int32> _pointIndexsToKeep = new List<Int32>(128);
        /// <summary>
        ///     Reduces the number of points
        /// </summary>
        public static void Reduce(List<MapPoint> source, List<MapPoint> destination, Double tolerance)
        {
            if (source == null || source.Count < 3)
            {
                destination.AddRange(source);
                return;
            }

            Int32 firstPoint = 0;
            Int32 lastPoint = source.Count - 1;
            _pointIndexsToKeep.Clear();

            //Add the first and last index to the keepers
            _pointIndexsToKeep.Add(firstPoint);
            _pointIndexsToKeep.Add(lastPoint);

            //The first and the last point can not be the same
            while (source[firstPoint].Equals(source[lastPoint]))
                lastPoint--;

            Reduce(source, firstPoint, lastPoint, tolerance, ref _pointIndexsToKeep);

            _pointIndexsToKeep.Sort();

            foreach (Int32 index in _pointIndexsToKeep)
                destination.Add(source[index]);
        }

        /// <summary>
        ///     Douglases the peucker reduction.
        /// </summary>
        private static void Reduce(List<MapPoint> source, Int32 firstPoint, Int32 lastPoint, Double tolerance,
            ref List<Int32> pointIndexsToKeep)
        {
            Double maxDistance = 0;
            Int32 indexFarthest = 0;

            for (Int32 index = firstPoint; index < lastPoint; index++)
            {
                Double distance = PerpendicularDistance(source[firstPoint], source[lastPoint], source[index]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    indexFarthest = index;
                }
            }

            if (maxDistance > tolerance && indexFarthest != 0)
            {
                //Add the largest point that exceeds the tolerance
                pointIndexsToKeep.Add(indexFarthest);

                Reduce(source, firstPoint, indexFarthest, tolerance, ref pointIndexsToKeep);
                Reduce(source, indexFarthest, lastPoint, tolerance, ref pointIndexsToKeep);
            }
        }

        /// <summary>
        ///     The distance of a point from a line made from point1 and point2.
        /// </summary>
        public static Double PerpendicularDistance(MapPoint Point1, MapPoint Point2, MapPoint Point)
        {
            Double area =
                Math.Abs(.5*
                         (Point1.X*Point2.Y + Point2.X*Point.Y + Point.X*Point1.Y - Point2.X*Point1.Y - Point.X*Point2.Y -
                          Point1.X*Point.Y));
            Double bottom = Math.Sqrt(Math.Pow(Point1.X - Point2.X, 2) + Math.Pow(Point1.Y - Point2.Y, 2));
            Double height = area/bottom*2;

            return height;
        }
    }
}