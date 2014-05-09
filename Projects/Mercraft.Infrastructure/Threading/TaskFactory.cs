using System;
using System.Collections;

namespace Mercraft.Infrastructure.Threading
{
    public class TaskFactory
    {
        public Task Create(Action<Task> action)
        {
            return new Task<Task.Unit>(action);
        }

        public Task Create(Action action)
        {
            return new Task<Task.Unit>(action);
        }

        public Task<T> Create<T>(Func<Task, T> func)
        {
            return new Task<T>(func);
        }

        public Task<T> Create<T>(Func<T> func)
        {
            return new Task<T>(func);
        }

        public Task Create(IEnumerator enumerator)
        {
            return new Task<IEnumerator>(enumerator);
        }

        public Task<T> Create<T>(Type type, string methodName, params object[] args)
        {
            return new Task<T>(type, methodName, args);
        }

        public Task<T> Create<T>(object that, string methodName, params object[] args)
        {
            return new Task<T>(that, methodName, args);
        }
    }
}
