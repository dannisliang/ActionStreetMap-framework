
namespace Mercraft.Maps.Core.Collections.LongIndex
{
    /// <summary>
    /// An efficient index for OSM object ids.
    /// </summary>
    public class LongIndex
    {
        /// <summary>
        /// The root of the positive ids.
        /// </summary>
        private LongIndexNode _rootPositive;

        /// <summary>
        /// The root of the negative ids.
        /// </summary>
        private LongIndexNode _rootNegative;

        /// <summary>
        /// Creates a new longindex.
        /// </summary>
        public LongIndex()
        {
            _rootPositive = new LongIndexNode(1);
            _rootNegative = new LongIndexNode(1);
        }

        /// <summary>
        /// Adds an id.
        /// </summary>
        /// <param name="number"></param>
        public void Add(long number)
        {
            if (number >= 0)
            {
                this.PositiveAdd(number);
            }
            else
            {
                this.NegativeAdd(-number);
            }
        }

        /// <summary>
        /// Removes an id.
        /// </summary>
        /// <param name="number"></param>
        public void Remove(long number)
        {
            if (number >= 0)
            {
                this.PositiveRemove(number);
            }
            else
            {
                this.NegativeAdd(-number);
            }
        }

        /// <summary>
        /// Returns true if the id is there.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public bool Contains(long number)
        {
            if (number >= 0)
            {
                return this.PositiveContains(number);
            }
            else
            {
                return this.NegativeContains(-number);
            }
        }

        #region Positive

        /// <summary>
        /// Adds an id.
        /// </summary>
        /// <param name="number"></param>
        private void PositiveAdd(long number)
        {
            while (number >= LongIndexNode.CalculateBaseNumber((short)(_rootPositive.Base + 1)))
            {
                LongIndexNode oldRoot = _rootPositive;
                _rootPositive = new LongIndexNode((short)(_rootPositive.Base + 1));
                _rootPositive.Has0 = oldRoot;
            }

            _rootPositive.Add(number);
        }

        /// <summary>
        /// Removes an id.
        /// </summary>
        /// <param name="number"></param>
        private void PositiveRemove(long number)
        {
            if (number < LongIndexNode.CalculateBaseNumber((short)(_rootPositive.Base + 1)))
            {
                _rootPositive.Remove(number);
            }
        }

        /// <summary>
        /// Returns true if the id is there.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private bool PositiveContains(long number)
        {
            if (number < LongIndexNode.CalculateBaseNumber((short)(_rootPositive.Base + 1)))
            {
                return _rootPositive.Contains(number);
            }
            return false;
        }

        #endregion

        #region Negative

        /// <summary>
        /// Adds an id.
        /// </summary>
        /// <param name="number"></param>
        private void NegativeAdd(long number)
        {
            while (number >= LongIndexNode.CalculateBaseNumber((short)(_rootNegative.Base + 1)))
            {
                LongIndexNode oldRoot = _rootNegative;
                _rootNegative = new LongIndexNode((short)(_rootNegative.Base + 1));
                _rootNegative.Has0 = oldRoot;
            }

            _rootNegative.Add(number);
        }

        /// <summary>
        /// Removes an id.
        /// </summary>
        /// <param name="number"></param>
        private void NegativeRemove(long number)
        {
            if (number < LongIndexNode.CalculateBaseNumber((short)(_rootNegative.Base + 1)))
            {
                _rootNegative.Remove(number);
            }
        }

        /// <summary>
        /// Returns true if the id is there.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private bool NegativeContains(long number)
        {
            if (number < LongIndexNode.CalculateBaseNumber((short)(_rootNegative.Base + 1)))
            {
                return _rootNegative.Contains(number);
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Clears this index.
        /// </summary>
        public void Clear()
        {
            _rootPositive = new LongIndexNode(1);
            _rootNegative = new LongIndexNode(1);
        }    
    }
}