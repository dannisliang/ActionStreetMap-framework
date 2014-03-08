using Mercraft.Core.Utilities;

namespace Mercraft.Core
{
    public struct GeoCoordinate
    {
        /// <summary>
        /// Latitude in degrees
        /// </summary>
        public readonly double Latitude;

        /// <summary>
        /// Longitude in degrees
        /// </summary>
        public readonly double Longitude;

        public GeoCoordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public static bool operator ==(GeoCoordinate a, GeoCoordinate b)
        {
            return MathUtility.AreEqual(a.Latitude, b.Latitude) &&
                   MathUtility.AreEqual(a.Longitude, b.Longitude);
        }

        public static bool operator !=(GeoCoordinate a, GeoCoordinate b)
        {
            return !(a == b);
        }

        public override bool Equals(object other)
        {
            if (!(other is GeoCoordinate))
                return false;
            var coord = (GeoCoordinate)other;
            return MathUtility.AreEqual(Latitude, coord.Latitude) &&
                   MathUtility.AreEqual(Longitude, coord.Longitude);
        }

        public override int GetHashCode()
        {
            return Latitude.GetHashCode() ^ Longitude.GetHashCode() << 2;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", Latitude, Longitude);
        }
    }
}