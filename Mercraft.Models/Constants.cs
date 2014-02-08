
namespace Mercraft.Math
{
    /// <summary>
    /// Contains generic constants.
    /// </summary>
    public static class Constants
    {

        /// <summary>
        /// 2.0 * Math.PI
        /// </summary>
        public const double TwoPi = 6.283185307179586476925286766559;  // 2.0 * Math.PI;

        /// <summary>
        /// The number of seconds per hour.
        /// </summary>
        public const double SecondsPerHour = 3600.0;

        /// <summary>
        /// Regex to parse decimals.
        /// </summary>
        public const string RegexDecimal = @"(\d+(?:\.\d*)?)";

        /// <summary>
        /// Regex for whitespaces.
        /// </summary>
        public const string RegexDecimalWhiteSpace = @"\s*" + RegexDecimal + @"\s*";
    }
}