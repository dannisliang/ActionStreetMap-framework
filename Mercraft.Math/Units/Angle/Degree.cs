namespace Mercraft.Math.Units.Angle
{
    /// <summary>
    /// Represents an angle in degress.
    /// </summary>
    public class Degree : Unit
    {
        private Degree()
            : base(0.0d)
        {

        }

        /// <summary>
        /// Creates a new angle in degrees.
        /// </summary>
        /// <param name="value"></param>
        public Degree(double value)
            :base(Degree.Normalize(value))
        {

        }

		/// <summary>
		/// Normalize the specified value.
		/// </summary>
		/// <param name="value">Value.</param>
        private static double Normalize(double value)
        {
            int count360 = (int)System.Math.Floor(value / 360.0);
            return value - (count360 * 360.0);
        }

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Degree"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Degree"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("{0}°", this.Value);
		}

        /// <summary>
        /// Converts the given angle to the range -180, +180.
        /// </summary>
        /// <returns></returns>
        public double Range180()
        {
            if (this.Value > 180)
            {
                return this.Value - 360;
            }
            return this.Value;
        }

        /// <summary>
        /// Substracts the two angles returning an angle in the range -180, +180
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public double SmallestDifference(Degree angle)
        {
            return (this - angle).Range180();
        }

        #region Conversion

        /// <summary>
        /// Converts the given value to degrees.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Degree(double value)
        {
            return new Degree(value);
        }

        /// <summary>
        /// Converts the given value to degrees.
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static implicit operator Degree(Radian rad)
        {
            double value = (rad.Value / System.Math.PI) * 180d;
            return new Degree(value);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Subtracts two angles.
        /// </summary>
        /// <param name="deg1"></param>
        /// <param name="deg2"></param>
        /// <returns></returns>
        public static Degree operator -(Degree deg1, Degree deg2)
        {
            return deg1.Value - deg2.Value;
        }

        /// <summary>
        /// Adds two angles.
        /// </summary>
        /// <param name="deg1"></param>
        /// <param name="deg2"></param>
        /// <returns></returns>
        public static Degree operator +(Degree deg1, Degree deg2)
        {
            return deg1.Value + deg2.Value;
        }

        /// <summary>
        /// Returns the absolute value of the angle.
        /// </summary>
        /// <returns></returns>
        public Degree Abs()
        {
            return System.Math.Abs(this.Value);
        }

        /// <summary>
        /// Returns true if one angle is greater than the other.
        /// </summary>
        /// <param name="deg1"></param>
        /// <param name="deg2"></param>
        /// <returns></returns>
        public static bool operator >(Degree deg1,Degree deg2)
        {
            return deg1.Value > deg2.Value;
        }

        /// <summary>
        /// Returns true if one angle is smaller than the other.
        /// </summary>
        /// <param name="deg1"></param>
        /// <param name="deg2"></param>
        /// <returns></returns>
        public static bool operator <(Degree deg1, Degree deg2)
        {
            return deg1.Value < deg2.Value;
        }

        /// <summary>
        /// Returns true if one angle is greater or equal than the other.
        /// </summary>
        /// <param name="deg1"></param>
        /// <param name="deg2"></param>
        /// <returns></returns>
        public static bool operator >=(Degree deg1, Degree deg2)
        {
            return deg1.Value >= deg2.Value;
        }

        /// <summary>
        /// Returns true if one angle is smaller or equal than the other.
        /// </summary>
        /// <param name="deg1"></param>
        /// <param name="deg2"></param>
        /// <returns></returns>
        public static bool operator <=(Degree deg1, Degree deg2)
        {
            return deg1.Value <= deg2.Value;
        }

        #endregion
    }
}
