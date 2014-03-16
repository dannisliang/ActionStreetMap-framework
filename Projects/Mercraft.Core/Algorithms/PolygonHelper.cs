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
            var verticies = geoCoordinates
                .Select(g => GeoProjection.ToMapCoordinate(center, g))
                .ToArray();

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

        internal enum PolygonDirection
        {
            Unknown,
            Clockwise,
            CountClockwise
        }

    }
}
