using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        public static Vector2[] GetVerticies2D(GeoCoordinate center, IList<GeoCoordinate> geoCoordinates)
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

            var  verticies = geoCoordinates
                .Select(g => GeoProjection.ToMapCoordinate(center, g))
                .Take(length).ToArray();

            return SortVertices(verticies);
        }

        /// <summary>
        /// Sorts verticies in clockwise order
        /// </summary>
        private static Vector2[] SortVertices(Vector2[] verticies)
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

        private static PolygonDirection PointsDirection(Vector2[] points)
        {
            int nCount = 0, j = 0, k = 0;
            int nPoints = points.Length;

            if (nPoints < 3)
                return PolygonDirection.Unknown;

            for (int i = 0; i < nPoints; i++)
            {
                j = (i + 1) % nPoints; //j:=i+1;
                k = (i + 2) % nPoints; //k:=i+2;

                double crossProduct = (points[j].x - points[i].x)
                    * (points[k].y - points[j].y);
                crossProduct = crossProduct - (
                    (points[j].y - points[i].y)
                    * (points[k].x - points[j].x)
                    );

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

        public static Vector3[] GetVerticies(Vector2[] verticies2D, float floor)
        {
            var length = verticies2D.Length;
            var verticies3D = new Vector3[length];
            for (int i = 0; i < length; i++)
            {
                verticies3D[i] = new Vector3(verticies2D[i].x, floor, verticies2D[i].y);
            }

            return verticies3D;
        }


        public static Vector3[] GetVerticies3D(Vector2[] verticies2D, float top, float floor)
        {
            var length = verticies2D.Length;
            var verticies3D = new Vector3[length * 2];
            for (int i = 0; i < length; i++)
            {
                verticies3D[i] = new Vector3(verticies2D[i].x, floor, verticies2D[i].y);
                verticies3D[i + length] = new Vector3(verticies2D[i].x, top, verticies2D[i].y);
            }

            return verticies3D;
        }

        public static int[] GetTriangles(Vector2[] verticies2D)
        {
            var triangulator = new Triangulator(verticies2D);
            return triangulator.Triangulate();
        }

        // TODO optimization: we needn't triangles for floor in case of building!
        public static int[] GetTriangles3D(Vector2[] verticies2D)
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

        public static Vector2[] GetUV(Vector2[] verticies2D)
        {
            var length = verticies2D.Length;
            var uvs = new Vector2[length * 2];

            for (int i = 0; i < length; i++)
            {
                uvs[i] = new Vector2(verticies2D[i].x, verticies2D[i].y);
                uvs[i + length] = new Vector2(verticies2D[i].x, verticies2D[i].y);
            }

            return uvs;
        }

        internal enum PolygonDirection
        {
            Unknown,
            Clockwise,
            CountClockwise
        }

    }
}
