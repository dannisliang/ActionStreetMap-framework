
using System.Collections.Generic;

namespace Mercraft.Math.Primitives.Enumerators.Points
{
    /// <summary>
    /// An enumerable for a point.
    /// </summary>
    public sealed class PointEnumerable : IEnumerable<PointF2D>
    {
        private PointEnumerator _enumerator;

        internal PointEnumerable(PointEnumerator enumerator)
        {
            _enumerator = enumerator;
        }

        #region IEnumerable<PointType> Members

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<PointF2D> GetEnumerator()
        {
            return _enumerator;
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _enumerator;
        }

        #endregion
    }
}
