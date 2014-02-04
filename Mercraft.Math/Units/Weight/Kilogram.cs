
namespace Mercraft.Math.Units.Weight
{
    /// <summary>
    /// Represents a weight in kilograms.
    /// </summary>
    public class Kilogram : Unit
    {
        /// <summary>
        /// Creates a new kilogram.
        /// </summary>
        public Kilogram()
            : base(0.0d)
        {

        }

        private Kilogram(double value)
            : base(value)
        {

        }

        #region Conversions

        /// <summary>
        /// Converts a value to kilograms.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Kilogram(double value)
        {
            return new Kilogram(value);
        }

        /// <summary>
        /// Converts a value to kilograms.
        /// </summary>
        /// <param name="gram"></param>
        /// <returns></returns>
        public static implicit operator Kilogram(Gram gram)
        {
            return gram.Value / 1000d;
        }

        #endregion

        /// <summary>
        /// Returns a description of this kilogram.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value.ToString() + "Kg";
        }

    }
}
