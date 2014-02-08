using Mercraft.Models.Units.Speed;
using Mercraft.Models.Units.Time;

namespace Mercraft.Models.Units.Distance
{
    /// <summary>
    /// Represents a distance in meters.
    /// </summary>
    public class Meter : Unit
    {
        /// <summary>
        /// Creates a new meter.
        /// </summary>
        public Meter()
            :base(0.0d)
        {

        }

        private Meter(double value)
            : base(value)
        {

        }

        #region Conversions

        /// <summary>
        /// Converts the given value to meters.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Meter(double value)
        {
            return new Meter(value);
        }

        /// <summary>
        /// Converts the given value to meters.
        /// </summary>
        /// <param name="kilometer"></param>
        /// <returns></returns>
        public static implicit operator Meter(Kilometer kilometer)
        {
            return kilometer.Value * 1000d;
        }

        #endregion
        
        #region Division
        
        /// <summary>
        /// Divides the distance to a time into a speed.
        /// </summary>
        /// <param name="meter"></param>
        /// <param name="sec"></param>
        /// <returns></returns>
        public static MeterPerSecond operator /(Meter meter, Second sec)
        {
            return meter.Value / sec.Value;
        }

        /// <summary>
        /// Divides the distance to a speed into a time.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static Second operator /(Meter distance, MeterPerSecond speed)
        {
            return distance.Value / speed.Value;
        }

        /// <summary>
        /// Divides the distance to a speed into a time.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static Second operator /(Meter distance, KilometerPerHour speed)
        {
            Kilometer distance_km = distance;
            return distance_km / speed;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Adds two distances.
        /// </summary>
        /// <param name="meter1"></param>
        /// <param name="meter2"></param>
        /// <returns></returns>
        public static Meter operator +(Meter meter1, Meter meter2)
        {
            return meter1.Value + meter2.Value;
        }

        /// <summary>
        /// Subtracts two distances.
        /// </summary>
        /// <param name="meter1"></param>
        /// <param name="meter2"></param>
        /// <returns></returns>
        public static Meter operator -(Meter meter1, Meter meter2)
        {
            return meter1.Value - meter2.Value;
        }

        #endregion

        /// <summary>
        /// Returns a description of this meter.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value.ToString() + "m";
        }
    }
}
