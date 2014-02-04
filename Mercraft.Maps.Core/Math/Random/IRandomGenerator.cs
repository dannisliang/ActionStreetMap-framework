
namespace Mercraft.Maps.Core.Math.Random
{
    /// <summary>
    /// A representation of generic random generator functions
    /// </summary>
    public interface IRandomGenerator
    {
        /// <summary>
        /// Generates a random integer
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        int Generate(int max);

        /// <summary>
        /// Generates a random double
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        double Generate(double max);

        /// <summary>
        /// Sets the seed of the static generator.
        /// </summary>
        /// <param name="buffer"></param>
        void Generate(byte[] buffer);
    }
}
