using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercraft.Infrastructure.Threading
{
    internal class ListPartitioner<T> : OrderablePartitioner<T>
    {
        IList<T> source;

        public ListPartitioner(IList<T> source)
            : base(true, true, true)
        {
            this.source = source;
        }

        public override IList<IEnumerator<KeyValuePair<long, T>>> GetOrderablePartitions(int partitionCount)
        {
            if (partitionCount <= 0)
                throw new ArgumentOutOfRangeException("partitionCount");

            IEnumerator<KeyValuePair<long, T>>[] enumerators
                = new IEnumerator<KeyValuePair<long, T>>[partitionCount];

            int count = source.Count / partitionCount;
            int extra = 0;

            if (source.Count < partitionCount)
            {
                count = 1;
            }
            else
            {
                extra = source.Count % partitionCount;
                if (extra > 0)
                    ++count;
            }

            int currentIndex = 0;

            Range[] ranges = new Range[enumerators.Length];
            for (int i = 0; i < ranges.Length; i++)
            {
                ranges[i] = new Range(currentIndex,
                                       currentIndex + count);
                currentIndex += count;
                if (--extra == 0)
                    --count;
            }

            for (int i = 0; i < enumerators.Length; i++)
            {
                enumerators[i] = GetEnumeratorForRange(ranges, i);
            }

            return enumerators;
        }

        class Range
        {
            public int Actual;
            public readonly int LastIndex;

            public Range(int frm, int lastIndex)
            {
                Actual = frm;
                LastIndex = lastIndex;
            }
        }

        IEnumerator<KeyValuePair<long, T>> GetEnumeratorForRange(Range[] ranges, int workerIndex)
        {
            if (ranges[workerIndex].Actual >= source.Count)
                return GetEmpty();

            return GetEnumeratorForRangeInternal(ranges, workerIndex);
        }

        IEnumerator<KeyValuePair<long, T>> GetEmpty()
        {
            yield break;
        }

        IEnumerator<KeyValuePair<long, T>> GetEnumeratorForRangeInternal(Range[] ranges, int workerIndex)
        {
            Range range = ranges[workerIndex];
            int lastIndex = range.LastIndex;
            int index = range.Actual;

            for (int i = index; i < lastIndex; i = ++range.Actual)
            {
                yield return new KeyValuePair<long, T>(i, source[i]);
            }
        }
    }
}
