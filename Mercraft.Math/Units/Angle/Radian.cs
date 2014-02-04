
namespace Mercraft.Math.Units.Angle
{
    /// <summary>
    /// Represents an angle in radians.
    /// </summary>
    public class Radian : Unit
    {
        private Radian()
            : base(0.0d)
        {

        }

        /// <summary>
        /// Creates a new angle in radians.
        /// </summary>
        /// <param name="radians"></param>
        public Radian(double radians)
            : base(radians)
        {

        }

        #region Conversion

        /// <summary>
        /// Converts the given value to radians.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Radian(double value)
        {
            return new Radian(value);
        }

        /// <summary>
        /// Converts the given value to radians.
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static implicit operator Radian(Degree deg)
        {
            double value = (deg.Value / 180d) * System.Math.PI;
            return new Radian(value);
        }

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Radian"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Radian"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("{0} rad", this.Value);
		}

        #endregion

        /// <summary>
        /// Subtracts two radians.
        /// </summary>
        /// <param name="rad1"></param>
        /// <param name="rad2"></param>
        /// <returns></returns>
        public static Radian operator -(Radian rad1, Radian rad2)
        {
            return rad1.Value - rad2.Value;
        }
    }
}
