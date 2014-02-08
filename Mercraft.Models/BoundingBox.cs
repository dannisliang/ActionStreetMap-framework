
using System;
using System.Collections.Generic;

namespace Mercraft.Models
{
    /// <summary>
    /// Represents bounding box
    /// http://stackoverflow.com/questions/238260/how-to-calculate-the-bounding-box-for-a-given-lat-lng-location
    /// </summary>
    public class BoundingBox
    {
        public MapPoint MinPoint { get; set; }
        public MapPoint MaxPoint { get; set; }
        
        // Semi-axes of WGS-84 geoidal reference
        private const double WGS84_a = 6378137.0; // Major semiaxis [m]
        private const double WGS84_b = 6356752.3; // Minor semiaxis [m]

        public BoundingBox(MapPoint minPoint, MapPoint maxPoint)
        {
            MinPoint = minPoint;
            MaxPoint = maxPoint;
        }

        public bool Contains(double latitude, double longitude)
        {
            return (MaxPoint.Latitude > latitude && latitude >= MinPoint.Latitude) &&
                 (MaxPoint.Longitude > longitude && longitude >= MinPoint.Longitude);
        }

        public bool Contains(MapPoint point)
        {
            return Contains(point.Latitude, point.Longitude);
        }

        #region Operations

        /// <summary>
        /// Adds point to bounding boxes together yielding as result the smallest box that surrounds both.
        /// </summary>
        public static BoundingBox operator +(BoundingBox a, MapPoint b)
        {
            MapPoint minPoint = new MapPoint(
                a.MinPoint.Latitude < b.Latitude ? a.MinPoint.Latitude : b.Latitude,
                a.MinPoint.Longitude < b.Longitude ? a.MinPoint.Longitude : b.Longitude);

            MapPoint maxPoint = new MapPoint(
                a.MaxPoint.Latitude > b.Latitude ? a.MaxPoint.Latitude : b.Latitude,
                a.MaxPoint.Longitude > b.Longitude ? a.MaxPoint.Longitude : b.Longitude);

            return new BoundingBox(minPoint, maxPoint);
        }

        #endregion

        # region Creation

        /// <summary>
        /// Creates bounding box
        /// </summary>
        /// <param name="point">Center</param>
        /// <param name="halfSideInKm">Half length of the bounding box</param>
        /// <returns></returns>
        public static BoundingBox CreateBoundingBox(MapPoint point, double halfSideInKm)
        {
            // Bounding box surrounding the point at given coordinates,
            // assuming local approximation of Earth surface as a sphere
            // of radius given by WGS84
            var lat = Deg2rad(point.Latitude);
            var lon = Deg2rad(point.Longitude);
            var halfSide = 1000 * halfSideInKm;

            // Radius of Earth at given latitude
            var radius = WGS84EarthRadius(lat);
            // Radius of the parallel at given latitude
            var pradius = radius* Math.Cos(lat);

            var latMin = lat - halfSide/radius;
            var latMax = lat + halfSide/radius;
            var lonMin = lon - halfSide/pradius;
            var lonMax = lon + halfSide/pradius;

            return new BoundingBox(
                new MapPoint(Rad2deg(latMin), Rad2deg(lonMin)),
                new MapPoint(Rad2deg(latMax), Rad2deg(lonMax)));
        }

        // degrees to radians
        private static double Deg2rad(double degrees)
        {
            return Math.PI*degrees/180.0;
        }

        // radians to degrees
        private static double Rad2deg(double radians)
        {
            return 180.0*radians/Math.PI;
        }

        // Earth radius at a given latitude, according to the WGS-84 ellipsoid [m]
        private static double WGS84EarthRadius(double lat)
        {
            // http://en.wikipedia.org/wiki/Earth_radius
            var An = WGS84_a*WGS84_a*Math.Cos(lat);
            var Bn = WGS84_b*WGS84_b*Math.Sin(lat);
            var Ad = WGS84_a*Math.Cos(lat);
            var Bd = WGS84_b*Math.Sin(lat);
            return Math.Sqrt((An*An + Bn*Bn)/(Ad*Ad + Bd*Bd));
        }

        #endregion
    }
}
