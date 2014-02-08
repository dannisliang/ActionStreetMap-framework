
namespace Mercraft.Models.Units.Speed
{
    /// <summary>
    /// Represents a speed.
    /// </summary>
    public abstract class Speed : Unit
    {                
        /// <summary>
        /// Creates a new speed.
        /// </summary>
        internal Speed(double value)
            :base(value)
        {

        }

        #region Parsers

        /// <summary>
        /// Tries to parse a string representing a speed. Assumes kilometers per hour by default, use explicit parsing methods for different behaviour.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string s, out Speed result)
        {
            result = null;

            if (string.IsNullOrWhiteSpace(s))
                return false;

            // try a generic parse first, in this case assume kilometers per hour.
            double value;
            if (double.TryParse(s, out value))
            { // the value is just a numeric value.
                result = new KilometerPerHour(value);
                return true;
            }

            // try kilometers per hour.
            if (KilometerPerHour.TryParse(s, out result))
            { // succes!
                return true;
            }

            // try miles per hour.
            if (MilesPerHour.TryParse(s, out result))
            { // success!
                return true;
            }

            // try knots.
            if (Knots.TryParse(s, out result))
            { // success!
                return true;
            }

            // try meters per second.
            if (MeterPerSecond.TryParse(s, out result))
            { // success!
                return true;
            }
            return false;
        }

        #endregion Parsers
    }
}
