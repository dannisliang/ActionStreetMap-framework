
namespace Mercraft.Math.Primitives.Enumerators.Lines
{
    /// <summary>
    /// Interface representing a list of lines.
    /// </summary>
    internal interface ILineList
    {
        /// <summary>
        /// Returns the count of lines.
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// Returns the line at the given idx.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        LineF2D this[int idx]
        {
            get;
        }
    }
}
