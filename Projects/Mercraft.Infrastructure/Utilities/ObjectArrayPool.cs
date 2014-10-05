using System;
using System.Collections.Generic;

namespace Mercraft.Infrastructure.Utilities
{
    /// <summary>
    ///     Provides pool of arrays of certain size
    /// </summary>
    public class ObjectArrayPool<T>
    {
        private readonly Stack<T[]> _objectStack;
        private readonly int _arraySize;

        public ObjectArrayPool(int initialBufferSize, int arraySize)
        {
            _objectStack = new Stack<T[]>(initialBufferSize);
            _arraySize = arraySize;
        }

        public T[] New()
        {
            if (_objectStack.Count > 0)
            {
                var array = _objectStack.Pop();
                Array.Clear(array, 0, _arraySize);
                return array;
            }
            return new T[_arraySize];
        }

        public void Store(T[] array)
        {
            if (array.Length != _arraySize)
                throw new ArgumentException(String.Format("Array length should be {0}", _arraySize));
            _objectStack.Push(array);
        }
    }
}
