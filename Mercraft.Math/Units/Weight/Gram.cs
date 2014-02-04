
namespace Mercraft.Math.Units.Weight
{
    /// <summary>
    /// Represents a weight in grams.
    /// </summary>
    public class Gram : Unit
    {        
        /// <summary>
        /// Creates a new weight.
        /// </summary>
        public Gram()
            : base(0.0d)
        {

        }

        private Gram(double value)
            : base(value)
        {

        }

        #region Conversions

        /// <summary>
        /// Converts a value to grams.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Gram(double value)
        {
            return new Gram(value);
        }

        /// <summary>
        /// Converts a value to grams.
        /// </summary>
        /// <param name="kilogram"></param>
        /// <returns></returns>
        public static implicit operator Gram(Kilogram kilogram)
        {
            return kilogram.Value * 1000d;
        }

        #endregion

        /// <summary>
        /// Returns a description of this weight.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value.ToString() + "g";
        }
        
    }
}
