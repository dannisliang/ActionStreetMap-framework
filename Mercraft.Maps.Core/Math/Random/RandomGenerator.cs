namespace Mercraft.Maps.Core.Math.Random
{
    /// <summary>
    /// Random number generator.
    /// </summary>
    public class RandomGenerator : IRandomGenerator
    {
        private System.Random _random;

        /// <summary>
        /// Creates a new random generator.
        /// </summary>
        public RandomGenerator()
        {
            _random = new System.Random();
        }

        /// <summary>
        /// Creates a new random generator.
        /// </summary>
        public RandomGenerator(int seed)
        {
            _random = new System.Random(seed);
        }

        #region IRandomGenerator Members

        /// <summary>
        /// Generates a random number.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public int Generate(int max)
        {
            return _random.Next(max);
        }

        /// <summary>
        /// Generates a random number.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public double Generate(double max)
        {
            return _random.NextDouble() * max;
        }

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="buffer"></param>
        public void Generate(byte[] buffer)
        {
            _random.NextBytes(buffer);
        }

        #endregion
    }
}
