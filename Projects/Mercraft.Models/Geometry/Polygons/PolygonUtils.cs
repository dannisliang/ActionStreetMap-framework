using System;
using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Core.Elevation;

namespace Mercraft.Models.Geometry.Polygons
{
    /// <summary>
    ///     Provides some polygon util methods.
    /// </summary>
    public static class PolygonUtils
    {
        /// <summary>
        ///     Cuts given polygon by tile corners.
        /// </summary>
        /// <param name="leftBottom">Left bottom corner.</param>
        /// <param name="rightUpper">Right upper corner.</param>
        /// <param name="points">Closed polygon points.</param>
        public static void ClipPolygonByTile(MapPoint leftBottom, MapPoint rightUpper, List<MapPoint> points)
        {
            // TODO Optimize memory allocations for lists
            var result = PolygonClipper.GetIntersectedPolygon(points,
                new List<MapPoint>()
                {
                    leftBottom,
                    new MapPoint(leftBottom.X, rightUpper.Y),
                    rightUpper,
                    new MapPoint(rightUpper.X, leftBottom.Y),
                });
            points.Clear();
            points.AddRange(result);
        }

        /// <summary>
        ///     Cuts given polygon by tile corners.
        /// </summary>
        /// <param name="heightmap">Height map.</param>
        /// <param name="points">Closed polygon points.</param>
        public static void ClipPolygonByTile(HeightMap heightmap, List<MapPoint> points)
        {
            ClipPolygonByTile(heightmap.LeftBottomCorner, heightmap.RightUpperCorner, points);
            // set height
            for (int i = 0; i < points.Count; i++)
                points[i].SetElevation(heightmap.LookupHeight(points[i]));
        }

        /// <summary>
        ///     Make offset polygon
        /// </summary>
        /// <param name="verticies">Source polygon.</param>
        /// <param name="result">Result polygon.</param>
        /// <param name="offset">Offset.</param>
        public static void MakeOffset(List<MapPoint> verticies, List<MapPoint> result, float offset)
        {
            var polygon = new Polygon(verticies);
            for (int i = 0; i < polygon.Segments.Length; i++)
            {
                var previous = i == 0 ? polygon.Segments.Length - 1 : i - 1;

                var segment1 = polygon.Segments[previous];
                var segment2 = polygon.Segments[i];

                var parallel1 = SegmentUtils.GetParallel(segment1, offset);
                var parallel2 = SegmentUtils.GetParallel(segment2, offset);

                var ip1 = SegmentUtils.IntersectionPoint(parallel1, parallel2);

                // NOTE: Looks like a bug in this or verticies producer algorithm
                if (!float.IsNaN(ip1.x) && !float.IsNaN(ip1.z))
                    result.Add(new MapPoint(ip1.x, ip1.z));
            }
        }

        /// <summary>
        ///     Calcs center of polygon.
        /// </summary>
        /// <param name="polygon">Polygon.</param>
        /// <returns>Center of polygon.</returns>
        public static MapPoint GetCentroid(List<MapPoint> polygon)
        {
            float centroidX = 0.0f;
            float centroidY = 0.0f;

            for (int i = 0; i < polygon.Count; i++)
            {
                centroidX += polygon[i].X;
                centroidY += polygon[i].Y;
            }
            centroidX /= polygon.Count;
            centroidY /= polygon.Count;

            return (new MapPoint(centroidX, centroidY));
        }
    }
}
