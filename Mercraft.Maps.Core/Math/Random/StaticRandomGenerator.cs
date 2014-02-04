
namespace Mercraft.Maps.Core.Math.Random
{
    /// <summary>
    /// A static random generator.
    /// </summary>
    public static class StaticRandomGenerator
    {
        private static IRandomGenerator _generator;

        /// <summary>
        /// Returns a random number generator.
        /// </summary>
        /// <returns></returns>
        public static IRandomGenerator Get()
        {
            if (_generator == null)
            {
                _generator = new RandomGenerator();
            }
            return _generator;
        }

        /// <summary>
        /// Sets the seed of the static generator.
        /// </summary>
        /// <param name="seed"></param>
        public static void Set(int seed)
        {
            _generator = new RandomGenerator(seed);
        }
    }
}
