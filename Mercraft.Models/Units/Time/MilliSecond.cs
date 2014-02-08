using System;

namespace Mercraft.Models.Units.Time
{
    /// <summary>
    /// Represents a unit of time in milliseconds.
    /// </summary>
    public class MilliSecond : Unit
    {
        /// <summary>
        /// Creates a new millisecond.
        /// </summary>
        public MilliSecond()
            : base(0.0d)
        {

        }

        private MilliSecond(double value)
            : base(value)
        {

        }

        #region Conversions

        /// <summary>
        /// Converts the given value to seconds.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator MilliSecond(double value)
        {
            MilliSecond sec = new MilliSecond(value);
            return sec;
        }

        /// <summary>
        /// Converts the given value to seconds.
        /// </summary>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public static implicit operator MilliSecond(TimeSpan timespan)
        {
            MilliSecond sec = new MilliSecond();
            sec = timespan.TotalMilliseconds;
            return sec;
        }

        /// <summary>
        /// Converts the given value to seconds.
        /// </summary>
        /// <param name="hour"></param>
        /// <returns></returns>
        public static implicit operator MilliSecond(Hour hour)
        {
            MilliSecond sec = new MilliSecond();
            sec = hour.Value * 3600.0 * 1000.0;
            return sec;
        }

        #endregion

        /// <summary>
        /// Returns a description of this seconds.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value.ToString() + "ms";
        }
    }
}