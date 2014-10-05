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
                var list = _objectStack.Pop();
                list.Clear();
                return list;
            }
            return new List<T>(_listSize);
        }

        public void Store(List<T> list)
        {
            _objectStack.Push(list);
        }
    }
}
