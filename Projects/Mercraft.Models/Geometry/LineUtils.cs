using System;
using System.Collections.Generic;
using Mercraft.Core;

namespace Mercraft.Models.Geometry
{
    public static class LineUtils
    {
        public static MapPoint[] GetIntermediatePoints(MapPoint[] original, float maxDistance)
        {
            var result = new List<MapPoint>(original.Length);
            for (int i = 1; i < original.Length; i++)
            {
                var point1 = original[i - 1];
                var point2 = original[i];

                result.Add(point1);

                if (Math.Abs(point1.Elevation - point2.Elevation) > 0.1f)
                {
                    var distance = point1.DistanceTo(point2);
                    while (distance > maxDistance)
                    {
                        var ration = maxDistance/distance;
                        point1 = new MapPoint(
                            point1.X + ration*(point2.X - point1.X),
                            point1.Y + ration*(point2.Y - point1.Y),
                            point1.Elevation);
                        distance = point1.DistanceTo(point2);
                        result.Add(point1);
                    }
                }
            }
            // add last as we were looking at previous item in cycle
            result.Add(original[original.Length - 1]);
            return result.ToArray();
        }

        public static MapPoint GetNextIntermediatePoint(MapPoint point1, MapPoint point2, float maxDistance)
        {
            var next = point1;
            if (Math.Abs(point1.Elevation - point2.Elevation) > 0.1f)
            {
                var distance = point1.DistanceTo(point2);
                var ration = maxDistance / distance;
                next = new MapPoint(
                        point1.X + ration * (point2.X - point1.X),
                        point1.Y + ration * (point2.Y - point1.Y),
                        point1.Elevation);
            }

            return next;
        }
    }
}
