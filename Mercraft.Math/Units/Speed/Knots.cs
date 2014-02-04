using System.Globalization;
using System.Text.RegularExpressions;
using Mercraft.Math.Units.Distance;
using Mercraft.Math.Units.Time;

namespace Mercraft.Math.Units.Speed
{
    /// <summary>
    /// Represents a speed in knots.
    /// </summary>
    public class Knots : Speed
    {
        private const string RegexUnitKnots = @"\s*(knots)\s*";

        /// <summary>
        /// Creates a new knots.
        /// </summary>
        /// <param name="value"></param>
        public Knots(double value)
            : base(value)
        {

        }

        #region Conversions

        /// <summary>
        /// Converts a given value to knots.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Knots(double value)
        {
            return new Knots(value);
        }

        /// <summary>
        /// Converts a given value to knots.
        /// </summary>
        /// <param name="meterPerSec"></param>
        /// <returns></returns>
        public static implicit operator Knots(MeterPerSecond meterPerSec)
        {
            return meterPerSec.Value / 0.5144444444444444;
        }

        /// <summary>
        /// Converts a given value to knots.
        /// </summary>
        /// <param name="knot"></param>
        /// <returns></returns>
        public static implicit operator Knots(KilometerPerHour knot)
        {
            return knot.Value / 1.85200;
        }

        /// <summary>
        /// Converts a given value to knots.
        /// </summary>
        /// <param name="mph"></param>
        /// <returns></returns>
        public static implicit operator Knots(MilesPerHour mph)
        {
            return mph.Value / 1.150779;
        }

        /// <summary>
        /// Divides a distance to a time resulting in a speed.
        /// </summary>
        /// <param name="knot"></param>
        /// <param name="hour"></param>
        /// <returns></returns>
        public static Kilometer operator *(Knots knot, Hour hour)
        {
            return ((KilometerPerHour)knot) * hour;
        }

        #endregion

        #region Parsing

        /// <summary>
        /// Tries to parse a string containing a meters per second value.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string s, out Knots result)
        {
            result = null;
            double value;
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            { // the value is just a numeric value.
                result = new Knots(value);
                return true;
            }

            // do some more parsing work.
            Regex regex = new Regex("^" + Constants.RegexDecimalWhiteSpace + Knots.RegexUnitKnots + "$", RegexOptions.IgnoreCase);
            Match match = regex.Match(s);
            if (match.Success)
            {
                result = new Knots(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture));
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
            return this.Value.ToString(CultureInfo.InvariantCulture) + "knots";
        }
    }
}
