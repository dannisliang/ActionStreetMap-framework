
using System.Collections.Generic;

namespace Mercraft.Math.Primitives.Enumerators.Lines
{
    /// <summary>
    /// An enurable for a line.
    /// </summary>
    public sealed class LineEnumerable : IEnumerable<LineF2D>
    {
        private LineEnumerator _enumerator;

        internal LineEnumerable(LineEnumerator enumerator)
        {
            _enumerator = enumerator;
        }

        #region IEnumerable<PointType> Members

        /// <summary>
        /// Creates the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<LineF2D> GetEnumerator()
        {
            return _enumerator;
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Creates the enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _enumerator;
        }

        #endregion
    }
}
