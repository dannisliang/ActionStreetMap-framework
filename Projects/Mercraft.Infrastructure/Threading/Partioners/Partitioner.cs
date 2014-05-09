using System;
using System.Collections.Generic;

namespace Mercraft.Infrastructure.Threading
{
    internal static class Partitioner
    {
        public static OrderablePartitioner<TSource> Create<TSource>(IEnumerable<TSource> source)
        {
            IList<TSource> tempIList = source as IList<TSource>;
            if (tempIList != null)
                return Create(tempIList, true);

            return new EnumerablePartitioner<TSource>(source);
        }

        public static OrderablePartitioner<TSource> Create<TSource>(TSource[] array, bool loadBalance)
        {
            return Create((IList<TSource>) array, loadBalance);
        }

        public static OrderablePartitioner<TSource> Create<TSource>(IList<TSource> list, bool loadBalance)
        {
            return new ListPartitioner<TSource>(list);
        }
    }

    public abstract class Partitioner<TSource>
    {
        protected Partitioner()
        {

        }

        public abstract IList<IEnumerator<TSource>> GetPartitions(int partitionCount);
    }
}
