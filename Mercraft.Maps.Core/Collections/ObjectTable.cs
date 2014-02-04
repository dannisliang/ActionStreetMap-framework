
using System;
using System.Collections.Generic;

namespace Mercraft.Maps.Core.Collections
{
    /// <summary>
    /// An object table containing and index of object to reduce memory usage by preventing duplicates.
    /// </summary>
    public class ObjectTable<Type>
    {
        /// <summary>
        /// The array containing all strings.
        /// </summary>
        private Type[] _strings;

        /// <summary>
        /// A dictionary containing the index of each string.
        /// </summary>
        private Dictionary<Type, uint> _reverseIndex;

        /// <summary>
        /// Holds the initial capacity and is also used as an allocation step.
        /// </summary>
        private int _initCapacity;

        /// <summary>
        /// Holds the next idx.
        /// </summary>
        private uint _nextIdx = 0;

        /// <summary>
        /// Creates a new string table.
        /// </summary>
        /// <param name="reverseIndex">The reverse index is enable if true.</param>
        public ObjectTable(bool reverseIndex)
            :this(reverseIndex, 1000)
        {

        }

        /// <summary>
        /// Creates a new string table.
        /// </summary>
        /// <param name="reverseIndex">The reverse index is enable if true.</param>
        /// <param name="initCapacity"></param>
        public ObjectTable(bool reverseIndex, int initCapacity)
        {
            _strings = new Type[initCapacity];
            _initCapacity = initCapacity;

            if (reverseIndex)
            {
                this.BuildReverseIndex();
            }
        }

        /// <summary>
        /// Clears all data from this object table.
        /// </summary>
        public void Clear()
        {
            _strings = new Type[_initCapacity];
            _nextIdx = 0;
            if (_reverseIndex != null)
            {
                _reverseIndex.Clear();
            }
        }

        #region Reverse Index

        /// <summary>
        /// Builds the reverse index.
        /// </summary>
        public void BuildReverseIndex()
        {
            _reverseIndex = new Dictionary<Type, uint>();
            for(uint idx = 0; idx < _strings.Length; idx++)
            {
                Type value = _strings[idx];
                if (value != null)
                {
                    _reverseIndex[value] = idx;
                }
            }
        }

        /// <summary>
        /// Drops the reverse index.
        /// </summary>
        public void DropReverseIndex()
        {
            _reverseIndex = null;
        }

        #endregion

        #region Table

        private uint AddString(Type value)
        {
            uint value_int = _nextIdx;

            if (_strings.Length <= _nextIdx)
            { // the string table is not big enough anymore.
                Array.Resize<Type>(ref _strings, _strings.Length + _initCapacity);
            }
            _strings[_nextIdx] = value;

            if (_reverseIndex != null)
            {
                _reverseIndex[value] = _nextIdx;
            }

            _nextIdx++;
            return value_int;
        }

        #endregion

        /// <summary>
        /// Returns the highest id in this object table.
        /// </summary>
        public uint Count
        {
            get
            {
                return _nextIdx;
            }
        }

        /// <summary>
        /// Returns an index for the given string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public uint Add(Type value)
        {
            uint valueInt;
            if (_reverseIndex != null)
            { // add string based on the reverse index, is faster.
                if (!_reverseIndex.TryGetValue(value, out valueInt))
                { // string was not found.
                    valueInt = this.AddString(value);
                }
            }
            else
            {
                int idx = Array.IndexOf<Type>(_strings, value); // this is O(n), a lot worse compared to the best-case O(1).
                if (idx < 0)
                { // string was not found.
                    valueInt = this.AddString(value);
                }
                else
                { // string was found.
                    valueInt = (uint)idx;
                }
            }
            return valueInt;
        }

        /// <summary>
        /// Returns a string given it's encoded index.
        /// </summary>
        /// <param name="valueIdx"></param>
        /// <returns></returns>
        public Type Get(uint valueIdx)
        {
            return _strings[valueIdx];
        }

        /// <summary>
        /// Returns a copy of all data in this object table.
        /// </summary>
        /// <returns></returns>
        public Type[] ToArray()
        {
            Type[] copy = new Type[_nextIdx];
            for (int idx = 0; idx < _nextIdx; idx++)
            {
                copy[idx] = _strings[idx];
            }
            return copy;
        }
    }
}