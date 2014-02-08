using System.Collections;
using System.Collections.Generic;

namespace Mercraft.Models.Primitives.Enumerators.Lines
{
    internal class LineEnumerator :
        IEnumerator<LineF2D>,
        IEnumerator
    {
        /// <summary>
        /// Holds the enumerable being enumerated.
        /// </summary>
        private ILineList _enumerable;

        /// <summary>
        /// Holds the current line.
        /// </summary>
        private LineF2D _current_line;

        /// <summary>
        /// Holds the current index.
        /// </summary>
        private int _current_idx;

        /// <summary>
        /// Creates a new enumerator.
        /// </summary>
        /// <param name="enumerable"></param>
        public LineEnumerator(ILineList enumerable)
        {
            _enumerable = enumerable;
        }

        #region IEnumerator<GenericLineF2D<PointType>> Members

        /// <summary>
        /// Returns the current enumerator.
        /// </summary>
        public LineF2D Current
        {
            get { return _current_line; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _current_line = null;
        }

        #endregion

        #region IEnumerator Members

        object IEnumerator.Current
        {
            get { return _current_line; }
        }

        public bool MoveNext()
        {
            _current_idx++;
            if (_current_idx < _enumerable.Count)
            {
                _current_line = _enumerable[_current_idx];
                return true;
            }
            return false;
        }

        public void Reset()
        {
            _current_idx = -1;
        }

        #endregion

        #region IEnumerator Members


        bool IEnumerator.MoveNext()
        {
            return this.MoveNext();
        }

        void IEnumerator.Reset()
        {
            this.Reset();
        }

        #endregion
    }
}
