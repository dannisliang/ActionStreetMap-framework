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

            if (geoCoordinates[0] == geoCoordinates[length - 1])
                length--;

            return geoCoordinates
                .Select(g => GeoProjection.ToMapCoordinate(center, g))
                .Take(length).ToArray();
        }

        /// <summary>
        /// Sorts verticies in clockwise order
        /// </summary>
        public static Vector2[] SortVertices(Vector2[] verticies)
        {
            var direction = PolygonTriangulation.Polygon.PointsDirection(verticies);

            switch (direction)
            {
                case PolygonTriangulation.PolygonDirection.Clockwise:
                    return verticies.Reverse().ToArray();
                case PolygonTriangulation.PolygonDirection.CountClockwise:
                    return verticies;
                default:
                    // TODO need to understand what to do
                    return verticies;
                    //throw new NotImplementedException("Need to sort vertices!");
            }
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


        // TODO optimization: we needn't triangles for floor in case of building!
        public static int[] GetTriangles(Vector2[] verticies2D)
        {
            var verticiesLength = verticies2D.Length;
            var indecies = PolygonTriangulation.GetTriangles(verticies2D);
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
    }
}
