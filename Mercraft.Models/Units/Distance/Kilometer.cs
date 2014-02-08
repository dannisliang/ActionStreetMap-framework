using Mercraft.Models.Units.Speed;
using Mercraft.Models.Units.Time;

namespace Mercraft.Models.Units.Distance
{
    /// <summary>
    /// Represents a distance in kilometers.
    /// </summary>
    public class Kilometer : Unit
    {
        /// <summary>
        /// Creates a new kilometer.
        /// </summary>
        public Kilometer()
            : base(0.0d)
        {

        }

        private Kilometer(double value)
            : base(value)
        {

        }

        #region Conversions

        /// <summary>
        /// Converts the given value to kilometers.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Kilometer(double value)
        {
            return new Kilometer(value);
        }

        /// <summary>
        /// Converts the given value to kilometers.
        /// </summary>
        /// <param name="meter"></param>
        /// <returns></returns>
        public static implicit operator Kilometer(Meter meter)
        {
            return meter.Value / 1000d;
        }

        #endregion
        
        #region Division

        /// <summary>
        /// Divides a distance to a speed resulting in a time.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static Hour operator /(Kilometer distance, KilometerPerHour speed)
        {
            return distance.Value / speed.Value;
        }

        /// <summary>
        /// Divides a distance to a time resulting in a speed.
        /// </summary>
        /// <param name="kilometer"></param>
        /// <param name="hour"></param>
        /// <returns></returns>
        public static KilometerPerHour operator /(Kilometer kilometer, Hour hour)
        {
            return kilometer.Value / hour.Value;
        }

        #endregion

        /// <summary>
        /// Returns a description of this kilometer.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value.ToString() + "Km";
        }
    }
}