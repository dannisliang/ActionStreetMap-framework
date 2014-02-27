namespace Mercraft.Core
{
    public class GeoCoordinate
    {
        /// <summary>
        /// Latitude in degrees
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude in degrees
        /// </summary>
        public double Longitude { get; set; } 

        public GeoCoordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}