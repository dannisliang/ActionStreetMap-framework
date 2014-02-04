using System.Globalization;
using System.Text.RegularExpressions;
using Mercraft.Math.Units.Distance;
using Mercraft.Math.Units.Time;

namespace Mercraft.Math.Units.Speed
{
    /// <summary>
    /// Represents a speed in kilometer per hours.
    /// </summary>
    public class KilometerPerHour : Speed
    {
        private const string RegexUnitKilometersPerHour = @"\s*(km/h|kmh|kph|kmph)?\s*";
        
        /// <summary>
        /// Creates a new kilometers per hour.
        /// </summary>
        /// <param name="value"></param>
        public KilometerPerHour(double value)
            : base(value)
        {

        }

        #region Conversions

        /// <summary>
        /// Converts a given value to kilometers per hour.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator KilometerPerHour(double value)
        {
            return new KilometerPerHour(value);
        }

        /// <summary>
        /// Converts a given value to kilometers per hour.
        /// </summary>
        /// <param name="meterPerSec"></param>
        /// <returns></returns>
        public static implicit operator KilometerPerHour(MeterPerSecond meterPerSec)
        {
            return meterPerSec.Value * 3.6d;
        }

        /// <summary>
        /// Converts a given value to kilometers per hour.
        /// </summary>
        /// <param name="knot"></param>
        /// <returns></returns>
        public static implicit operator KilometerPerHour(Knots knot)
        {
            return knot.Value * 1.85200;
        }

        /// <summary>
        /// Converts a given value to kilimeters per hour.
        /// </summary>
        /// <param name="mph"></param>
        /// <returns></returns>
        public static implicit operator KilometerPerHour(MilesPerHour mph)
        {
            return mph.Value / 0.621371192;
        }

        /// <summary>
        /// Divides a distance to a time resulting in a speed.
        /// </summary>
        /// <param name="kilometerPerHour"></param>
        /// <param name="hour"></param>
        /// <returns></returns>
        public static Kilometer operator *(KilometerPerHour kilometerPerHour, Hour hour)
        {
            return kilometerPerHour.Value * hour.Value;
        }

        #endregion

        #region Parsing

        /// <summary>
        /// Tries to parse a string containing a kilometer per hour value.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string s, out KilometerPerHour result)
        {
            s = s == null? string.Empty : s.Trim().ToLower();

            result = null;
            double value;
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            { // the value is just a numeric value.
                result = new KilometerPerHour(value);
                return true;
            }

            // do some more parsing work.
            Regex regex = new Regex("^" + Constants.RegexDecimalWhiteSpace + KilometerPerHour.RegexUnitKilometersPerHour + "$", RegexOptions.IgnoreCase);
            Match match = regex.Match(s);
            if (match.Success)
            {
                result = new KilometerPerHour(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture));
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
            return this.Value.ToString(CultureInfo.InvariantCulture) + "Km/h";
        }
    }
}
