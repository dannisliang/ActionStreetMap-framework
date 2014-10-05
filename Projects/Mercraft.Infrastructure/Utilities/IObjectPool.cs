
using System.Collections.Generic;

namespace Mercraft.Infrastructure.Utilities
{
    public interface IObjectPool
    {
        T New<T>();
        void Store<T>(T obj);

        List<T> NewList<T>();
        List<T> NewList<T>(int capacity);
        void Store<T>(List<T> list);

        void Shrink();
    }
}
