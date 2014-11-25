using System;
using ActionStreetMap.Core;

namespace ActionStreetMap.Tests
{
    public static class GeoCoordinateHelper
    {
        public const double EarthRadiusInMeters = 6367000;

        public static double ToRadian(double val) { return val * (Math.PI / 180); }
        public static double DiffRadian(double val1, double val2) { return ToRadian(val2) - ToRadian(val1); }

        public static double CalcDistance(GeoCoordinate coord1, GeoCoordinate coord2)
        {
            return CalcDistance(coord1.Latitude, coord1.Longitude, coord2.Latitude, coord2.Longitude);
        }

        public static double CalcDistance(double lat1, double lng1, double lat2, double lng2)
        {
            return EarthRadiusInMeters * 2 * Math.Asin(Math.Min(1, Math.Sqrt((Math.Pow(Math.Sin((DiffRadian(lat1, lat2)) / 2.0), 2.0) + Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) * Math.Pow(Math.Sin((DiffRadian(lng1, lng2)) / 2.0), 2.0)))));
        }
    }
}
