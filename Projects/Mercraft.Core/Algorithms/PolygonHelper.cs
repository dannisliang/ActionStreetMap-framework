using System;
using System.Collections.Generic;
using System.Linq;

namespace Mercraft.Core.Algorithms
{
    /// <summary>
    /// TODO rename class
    /// </summary>
    public static class PolygonHelper
    {
        /// <summary>
        /// Converts geo coordinates to map coordinates
        /// </summary>
        /// <param name="center">Map center</param>
        /// <param name="geoCoordinates">Geo coordinates</param>
        /// <returns></returns>
        public static MapPoint[] GetVerticies2D(GeoCoordinate center, IList<GeoCoordinate> geoCoordinates)
        {
            var length = geoCoordinates.Count;


            for (int i = 0; i < length - 1; i++)
            {
                if (geoCoordinates[i] == geoCoordinates[length - 1])
                {
                    length--;
                    break;
                }
            }

            //if (geoCoordinates[0] == geoCoordinates[length - 1])
            //    length--;

            var verticies = geoCoordinates
                .Select(g => GeoProjection.ToMapCoordinate(center, g))
                .Take(length).ToArray();

            return SortVertices(verticies);
        }

        /// <summary>
        /// Sorts verticies in clockwise order
        /// </summary>
        private static MapPoint[] SortVertices(MapPoint[] verticies)
        {
            var direction = PointsDirection(verticies);

            switch (direction)
            {
                case PolygonDirection.Clockwise:
                    return verticies.Reverse().ToArray();
                case PolygonDirection.CountClockwise:
                    return verticies;
                default:
                    // TODO need to understand what to do
                    return verticies;
                    //throw new NotImplementedException("Need to sort vertices!");
            }
        }

        private static PolygonDirection PointsDirection(MapPoint[] points)
        {
            int nCount = 0, j = 0, k = 0;
            int nPoints = points.Length;

            if (nPoints < 3)
                return PolygonDirection.Unknown;

            for (int i = 0; i < nPoints; i++)
            {
                j = (i + 1) % nPoints; //j:=i+1;
                k = (i + 2) % nPoints; //k:=i+2;

                double crossProduct = (points[j].X - points[i].X)
                    * (points[k].Y - points[j].Y);
                crossProduct = crossProduct - 
                    ((points[j].Y - points[i].Y)* (points[k].X - points[j].X));

                if (crossProduct > 0)
                    nCount++;
                else
                    nCount--;
            }

            if (nCount < 0)
                return PolygonDirection.CountClockwise;
            else if (nCount > 0)
                return PolygonDirection.Clockwise;
            else
                return PolygonDirection.Unknown;
        }


        public static int[] GetTriangles(MapPoint[] verticies2D)
        {
            var triangulator = new Triangulator(verticies2D);
            return triangulator.Triangulate();
        }

        // TODO optimization: we needn't triangles for floor in case of building!
        public static int[] GetTriangles3D(MapPoint[] verticies2D)
        {
            var verticiesLength = verticies2D.Length;
            
            var triangulator = new Triangulator(verticies2D);
            var indecies = triangulator.Triangulate();
            
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
