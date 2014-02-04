using System.Globalization;
using System.Text.RegularExpressions;
using Mercraft.Math.Units.Distance;
using Mercraft.Math.Units.Time;

namespace Mercraft.Math.Units.Speed
{
    /// <summary>
    /// Represents a speed in meters/seconds.
    /// </summary>
    public class MeterPerSecond : Speed
    {
        private const string RegexUnitMetersPerSecond = @"\s*(m/s)?\s*";
        
        /// <summary>
        /// Creates a new meters per second object.
        /// </summary>
        /// <param name="value"></param>
        private MeterPerSecond(double value)
            :base(value)
        {

        }

        #region Conversions

        /// <summary>
        /// Converts a value to meters per second.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator MeterPerSecond(double value)
        {
            MeterPerSecond sec = new MeterPerSecond(value);
            return sec;
        }

        /// <summary>
        /// Converts a given value to kilometers per hour.
        /// </summary>
        /// <param name="kph"></param>
        /// <returns></returns>
        public static implicit operator MeterPerSecond(KilometerPerHour kph)
        {
            return kph.Value / 3.6d;
        }

        /// <summary>
        /// Converts a given value to kilometers per hour.
        /// </summary>
        /// <param name="knot"></param>
        /// <returns></returns>
        public static implicit operator MeterPerSecond(Knots knot)
        {
            return knot.Value * 1.85200 / 3.6;
        }

        /// <summary>
        /// Converts a given value to kilimeters per hour.
        /// </summary>
        /// <param name="mph"></param>
        /// <returns></returns>
        public static implicit operator MeterPerSecond(MilesPerHour mph)
        {
            return mph.Value * 1.609344 / 3.6;
        }

        /// <summary>
        /// Divides a distance to a time resulting in a speed.
        /// </summary>
        /// <param name="meterPerSecond"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Meter operator *(MeterPerSecond meterPerSecond, Second second)
        {
            return meterPerSecond.Value * second.Value;
        }
        #endregion

        #region Parsing

        /// <summary>
        /// Tries to parse a string containing a meters per second value.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string s, out MeterPerSecond result)
        {
            result = null;
            double value;
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            { // the value is just a numeric value.
                result = new MeterPerSecond(value);
                return true;
            }

            // do some more parsing work.
            Regex regex = new Regex("^" + Constants.RegexDecimalWhiteSpace + MeterPerSecond.RegexUnitMetersPerSecond + "$", RegexOptions.IgnoreCase);
            Match match = regex.Match(s);
            if (match.Success)
            {
                result = new MeterPerSecond(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture));
                return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Returns a description of this speed.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value.ToString(CultureInfo.InvariantCulture) + "m/s";
        }
    }
}
