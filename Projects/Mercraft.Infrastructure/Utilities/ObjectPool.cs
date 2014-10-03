using System;
using System.Collections.Generic;

namespace Mercraft.Infrastructure.Utilities
{
    public class ObjectPool<T> where T : class, new()
    {
        private Stack<T> _objectStack;

        private Action<T> _resetAction;
        private Action<T> _onetimeInitAction;

        public ObjectPool(int initialBufferSize, Action<T>
            resetAction = null, Action<T> onetimeInitAction = null)
        {
            _objectStack = new Stack<T>(initialBufferSize);
            _resetAction = resetAction;
            _onetimeInitAction = onetimeInitAction;
        }

        public T New()
        {
            if (_objectStack.Count > 0)
            {
                T t = _objectStack.Pop();

                if (_resetAction != null)
                    _resetAction(t);

                return t;
            }
            else
            {
                T t = new T();

                if (_onetimeInitAction != null)
                    _onetimeInitAction(t);

                return t;
            }
        }

        public void Store(T obj)
        {
            _objectStack.Push(obj);
        }
    }
}
