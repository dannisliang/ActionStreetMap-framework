using System;
using System.Linq;
using Mercraft.Infrastructure.Primitives;
using UnityEngine;

namespace Mercraft.Core.Algorithms
{
    public static class CircleHelper
    {
        private const double ConvertionCoefficient = (6378137 * Math.PI) / 180;

        public static Tuple<float, Vector2> GetCircle(GeoCoordinate relativeNullPoint, GeoCoordinate[] points)
        {
            var minLat = points.Min(a => a.Latitude);
            var maxLat = points.Max(a => a.Latitude);

            var minLon = points.Min(a => a.Longitude);
            var maxLon = points.Max(a => a.Longitude);

            var centerLat = (float)(minLat + (maxLat - minLat) / 2);
            var centerLon = (float)(minLon + (maxLon - minLon) / 2);
            var sphereCenter = GeoProjection.ToMapCoordinate(relativeNullPoint,
                new GeoCoordinate(centerLat, centerLon));

            var diameter = (float)((maxLat - minLat) * ConvertionCoefficient);

            return new Tuple<float, Vector2>(diameter, sphereCenter);
        }
    }
}
