using System;

namespace Mercraft.Models.Units.Time
{
    /// <summary>
    /// Represents a unit of time in seconds.
    /// </summary>
    public class Second : Unit
    {
        /// <summary>
        /// Creates a new second.
        /// </summary>
        public Second()
            :base(0.0d)
        {

        }

        private Second(double value)
            : base(value)
        {

        }

        #region Conversions

        /// <summary>
        /// Converts the given value to seconds.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Second(double value)
        {
            Second sec = new Second(value);
            return sec;
        }

        /// <summary>
        /// Converts the given value to seconds.
        /// </summary>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public static implicit operator Second(TimeSpan timespan)
        {
            Second sec = new Second();
            sec = timespan.TotalMilliseconds / 1000.0d;
            return sec;
        }

        /// <summary>
        /// Converts the given value to seconds.
        /// </summary>
        /// <param name="hour"></param>
        /// <returns></returns>
        public static implicit operator Second(Hour hour)
        {
            Second sec = new Second();
            sec = hour.Value * 3600.0d;
            return sec;
        }

        #endregion
        
        /// <summary>
        /// Returns a description of this seconds.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value.ToString() + "s";
        }
    }
}
