using System;
using System.Collections.Generic;

namespace Mercraft.Infrastructure.Utilities
{
    public class ObjectListPool<T>
    {
        private readonly Stack<List<T>> _objectStack;
        private readonly int _listSize;

        public ObjectListPool(int initialBufferSize, int listSize)
        {
            _objectStack = new Stack<List<T>>(initialBufferSize);
            _listSize = listSize;
        }

        public List<T> New()
        {
            if (_objectStack.Count > 0)
            {
                var t = _objectStack.Pop();
                t.Clear();
                return t;
            }
            return new List<T>(_listSize);
        }

        public void Store(List<T> obj)
        {
            _objectStack.Push(obj);
        }
    }
}
