namespace Mercraft.Models
{
    public class MapPoint
    {
        /// <summary>
        /// Latitude in degrees
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude in degrees
        /// </summary>
        public double Longitude { get; set; } 

        public MapPoint(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}