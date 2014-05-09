using System;
using System.Collections.Generic;
using System.Threading;

namespace Mercraft.Infrastructure.Threading
{
    public sealed class Parallel
    {
        public static void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (body == null)
                throw new ArgumentNullException("body");

            ForEach<TSource>(Partitioner.Create(source), body);
        }

        public static void ForEach<TSource>(Partitioner<TSource> source, Action<TSource> body)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (body == null)
                throw new ArgumentNullException("body");

            ForEach(source.GetPartitions,  body);
        }

        private static void ForEach<TSource>(Func<int, IList<IEnumerator<TSource>>> enumerable,
            Action<TSource> body)
        {
            int num = ThreadBase.AvailableProcessors;
            var tasks = new Task[num];
            IList<IEnumerator<TSource>> slices = enumerable(num);

            int sliceIndex = -1;

            Action workerMethod = delegate
            {
                IEnumerator<TSource> slice =
                    slices[Interlocked.Increment(ref sliceIndex)];

                while (slice.MoveNext())
                {
                    body(slice.Current);
                }
            };

            for (int i = 0; i < num; i++)
            {
                tasks[i] = Task.Factory.Create(workerMethod);
                tasks[i].Run();
            }
            tasks.WaitAll();
        }
    } 
}
