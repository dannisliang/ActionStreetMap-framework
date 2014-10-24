using System;
using System.Collections.Generic;
using Mercraft.Core.Elevation;

namespace Mercraft.Core.Algorithms
{
    /// <summary>
    ///     Provids some helper methods for points
    /// </summary>
    public static class PointHelper
    {
        /// <summary>
        ///     Converts geo coordinates to map coordinates.
        /// </summary>
        /// <param name="center">Map center.</param>
        /// <param name="geoCoordinates">Geo coordinates.</param>
        /// <param name="verticies">Output points.</param>
        public static void GetVerticies2D(GeoCoordinate center, List<GeoCoordinate> geoCoordinates, 
            List<MapPoint> verticies)
        {
            var length = geoCoordinates.Count;

            /*for (int i = 0; i < length - 1; i++)
            {
                if (geoCoordinates[i] == geoCoordinates[length - 1])
                {
                    length--;
                    break;
                }
            }*/

            if (geoCoordinates[0] == geoCoordinates[length - 1])
                length--;

            for (int i = 0; i < length; i++)
            {
                var point = GeoProjection.ToMapCoordinate(center, geoCoordinates[i]);
                verticies.Add(point);
            }

            SortVertices(verticies);
        }

        /// <summary>
        ///     Fills verticies
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="heightMap">Heightmap.</param>
        /// <param name="geoCoordinates">Geo coordinates.</param>
        /// <param name="verticies">Verticies.</param>
        /// <param name="sort">True if verticies should be sorted</param>
        public static void GetVerticies3D(GeoCoordinate center, HeightMap heightMap,
            List<GeoCoordinate> geoCoordinates, List<MapPoint> verticies, bool sort = true)
        {
            var length = geoCoordinates.Count;

            /*for (int i = 0; i < length - 1; i++)
            {
                if (geoCoordinates[i] == geoCoordinates[length - 1])
                {
                    length--;
                    break;
                }
            }*/


            if (geoCoordinates[0] == geoCoordinates[length - 1])
                length--;

            FillHeight(center, heightMap, geoCoordinates, verticies, length);

            if(sort)
                SortVertices(verticies);
        }

        /// <summary>
        ///     Fills heighmap.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="heightMap">Heightmap.</param>
        /// <param name="geoCoordinates">Geo coordinates.</param>
        /// <param name="verticies">Verticies.</param>
        /// <param name="length">Length.</param>
        public static void FillHeight(GeoCoordinate center, HeightMap heightMap, List<GeoCoordinate> geoCoordinates,
            List<MapPoint> verticies, int length)
        {
            for (int i = 0; i < length; i++)
            {
                var point = GeoProjection.ToMapCoordinate(center, geoCoordinates[i]);
                point.Elevation = heightMap.LookupHeight(point);
                verticies.Add(point);
            }
        }

        /// <summary>
        ///     Tests whether points represent convex polygon.
        /// </summary>
        /// <param name="points">Polygon points.</param>
        /// <returns>True if polygon is convex.</returns>
        public static bool IsConvex(List<MapPoint> points)
        {
            int count = points.Count;
            if (count < 4)
                return true;
            bool sign = false;
            for (int i = 0; i < count; i++)
            {
                double dx1 = points[(i + 2)%count].X - points[(i + 1)%count].X;
                double dy1 = points[(i + 2)%count].Y - points[(i + 1)%count].Y;
                double dx2 = points[i].X - points[(i + 1)%count].X;
                double dy2 = points[i].Y - points[(i + 1)%count].Y;
                double crossProduct = dx1*dy2 - dy1*dx2;
                if (i == 0)
                    sign = crossProduct > 0;
                else if (sign != (crossProduct > 0))
                    return false;

            }
            return true;
        }

        /// <summary>
        ///     Sorts verticies in clockwise order.
        /// </summary>
        private static void SortVertices(List<MapPoint> verticies)
        {
            var direction = PointsDirection(verticies);

            switch (direction)
            {
                case PolygonDirection.CountClockwise:
                    verticies.Reverse();
                    break;
                case PolygonDirection.Clockwise:
                    break;
                default:
                    throw new AlgorithmException(Strings.BugInPolygonOrderAlgorithm);
            }
        }

        private static PolygonDirection PointsDirection(List<MapPoint> points)
        {
            if (points.Count < 3)
                return PolygonDirection.Unknown;

            // Calculate signed area
            // http://en.wikipedia.org/wiki/Shoelace_formula
            double sum = 0.0;
            for (int i = 0; i < points.Count; i++)
            {
                MapPoint v1 = points[i];
                MapPoint v2 = points[(i + 1) % points.Count];
                sum += (v2.X - v1.X) * (v2.Y + v1.Y);
            }
            return sum > 0.0 ? PolygonDirection.Clockwise : PolygonDirection.CountClockwise;
        }

        /// <summary>
        ///     Gets triangles.
        /// </summary>
        /// <param name="verticies2D">Verticies.</param>
        /// <returns>Triangles.</returns>
        public static int[] GetTriangles(List<MapPoint> verticies2D)
        {
            return Triangulator.Triangulate(verticies2D);
        }

        // TODO optimization: we needn't triangles for floor in case of building!
        /// <summary>
        ///     Gets triangles for 3D building.
        /// </summary>
        /// <param name="verticies2D">Verticies.</param>
        /// <returns>Triangles.</returns>
        public static int[] GetTriangles3D(List<MapPoint> verticies2D)
        {
            var verticiesLength = verticies2D.Count;
            
            var indecies = Triangulator.Triangulate(verticies2D);
            
            //var indecies = PolygonTriangulation.GetTriangles3D(verticies2D);
            
            var length = indecies.Length;

            // add top
            Array.Resize(ref indecies, length * 2);
            for (var i = 0; i < length; i++)
            {
                indecies[i + length] = indecies[i] + verticiesLength;
            }

            // process square faces
            var oldIndeciesLength = indecies.Length;
            var faceTriangleCount = verticiesLength * 6;
            Array.Resize(ref indecies, oldIndeciesLength + faceTriangleCount);

            int j = 0;
            for (var i = 0; i < verticiesLength; i++)
            {
                var nextPoint = i < (verticiesLength - 1) ? i + 1 : 0;
                indecies[i + oldIndeciesLength + j] = i;
                indecies[i + oldIndeciesLength + ++j] = nextPoint;
                indecies[i + oldIndeciesLength + ++j] = i + verticiesLength;

                indecies[i + oldIndeciesLength + ++j] = i + verticiesLength;
                indecies[i + oldIndeciesLength + ++j] = nextPoint;
                indecies[i + oldIndeciesLength + ++j] = nextPoint + verticiesLength;
            }

            return indecies;
        }

        internal enum PolygonDirection
        {
            Unknown,
            Clockwise,
            CountClockwise
        }

    }
}
