
namespace Mercraft.Math.Units
{
    /// <summary>
    /// Represents a value representing a unit value.
    /// </summary>
    public abstract class Unit
    {
        /// <summary>
        /// The value of the unit.
        /// </summary>
        private double _value;

        /// <summary>
        /// Creates a new valued unit.
        /// </summary>
        /// <param name="value"></param>
        protected Unit(double value)
        {
            _value = value;
        }

        /// <summary>
        /// Returns the value.
        /// </summary>
        public double Value
        {
            get
            {
                return _value;
            }
        }
    }
}
