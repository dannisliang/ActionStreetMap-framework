using System;

namespace Mercraft.Core.Positioning
{
    /// <summary>
    ///     Represent geo position which is produced by location services (e.g. GLONASS, GPS).
    /// </summary>
    public class GeoPosition
    {
        /// <summary>
        ///     Longitude.
        /// </summary>
        public double Longitude = 0.0;

        /// <summary>
        ///     Latitude.
        /// </summary>
        public double Latitude = 0.0;

        /// <summary>
        ///     Speed in km/h
        /// </summary>
        public double Speed = -1.0;

        /// <summary>
        ///     Course in degrees
        /// </summary>
        public double Course = -1.0;

        /// <summary>
        ///     Date.
        /// </summary>
        public DateTime Date = DateTime.MinValue;

        /// <summary>
        ///     Time.
        /// </summary>
        public TimeSpan Time = TimeSpan.MinValue;

        /// <summary>
        ///     DateTime.
        /// </summary>
        public DateTime DateTime
        {
            get
            {
                return new DateTime(Date.Year, Date.Month, Date.Day, Time.Hours, Time.Minutes, Time.Seconds,
                    Time.Milliseconds);
            }
        }

        /// <summary>
        ///     Position fix.
        /// </summary>
        public int PositionFixIndicator = -1;

        /// <summary>
        ///     Satelites.
        /// </summary>
        public int Satelites = 0;

        /// <summary>
        ///     Hdop.
        /// </summary>
        public int Hdop = 0;

        /// <inheritdoc />
        public override string ToString()
        {
            return "Position: {Longitude=" + Longitude + " Latitude=" + Latitude
                   + "} S:" + Speed + "km/h C:" + Course
                   + " H:" + Hdop
                   + " P:" + PositionFixIndicator
                   + " ST: " + Satelites
                   + " D:" + DateTime;
        }
    }
}