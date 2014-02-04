
using System;

namespace Mercraft.Math.Units.Time
{
    /// <summary>
    /// Represents a unit of time in hours.
    /// </summary>
    public class Hour : Unit
    {
        /// <summary>
        /// Creates a new hour.
        /// </summary>
        public Hour()
            : base(0.0d)
        {

        }

        private Hour(double value)
            : base(value)
        {

        }

        #region Time-Conversions

        /// <summary>
        /// Converts a value to an hour.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Hour(double value)
        {
            Hour hr = new Hour(value);
            return hr;
        }

        /// <summary>
        /// Converts a value to an hour.
        /// </summary>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public static implicit operator Hour(TimeSpan timespan)
        {
            Hour hr = new Hour();
            hr = timespan.TotalMilliseconds * 1000.0d * 3600.0d;
            return hr;
        }

        /// <summary>
        /// Converts a value to an hour.
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public static implicit operator Hour(Second sec)
        {
            Hour hr = new Hour();
            hr = sec.Value / 3600.0d;
            return hr;
        }

        #endregion

        /// <summary>
        /// Returns a description of this hour.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value.ToString() + "H";
        }
    }
}
