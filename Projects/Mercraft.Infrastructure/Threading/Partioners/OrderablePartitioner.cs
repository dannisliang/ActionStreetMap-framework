
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mercraft.Infrastructure.Threading
{
    internal abstract class OrderablePartitioner<TSource> : Partitioner<TSource>
    {
        bool keysOrderedInEachPartition;
        bool keysOrderedAcrossPartitions;
        bool keysNormalized;

        protected OrderablePartitioner(bool keysOrderedInEachPartition,
                                        bool keysOrderedAcrossPartitions,
                                        bool keysNormalized)
            : base()
        {
            this.keysOrderedInEachPartition = keysOrderedInEachPartition;
            this.keysOrderedAcrossPartitions = keysOrderedAcrossPartitions;
            this.keysNormalized = keysNormalized;
        }

        public override IList<IEnumerator<TSource>> GetPartitions(int partitionCount)
        {
            IEnumerator<TSource>[] temp = new IEnumerator<TSource>[partitionCount];
            IList<IEnumerator<KeyValuePair<long, TSource>>> enumerators
              = GetOrderablePartitions(partitionCount);

            for (int i = 0; i < enumerators.Count; i++)
                temp[i] = new ProxyEnumerator(enumerators[i]);

            return temp;
        }


        IEnumerator<TSource> GetProxyEnumerator(IEnumerator<KeyValuePair<long, TSource>> enumerator)
        {
            while (enumerator.MoveNext())
                yield return enumerator.Current.Value;
        }

        public abstract IList<IEnumerator<KeyValuePair<long, TSource>>> GetOrderablePartitions(int partitionCount);

        
        public bool KeysOrderedInEachPartition
        {
            get
            {
                return keysOrderedInEachPartition;
            }
        }

        public bool KeysOrderedAcrossPartitions
        {
            get
            {
                return keysOrderedAcrossPartitions;
            }
        }

        public bool KeysNormalized
        {
            get
            {
                return keysNormalized;
            }
        }

        class ProxyEnumerator : IEnumerator<TSource>, IDisposable
        {
            IEnumerator<KeyValuePair<long, TSource>> internalEnumerator;

            internal ProxyEnumerator(IEnumerator<KeyValuePair<long, TSource>> enumerator)
            {
                internalEnumerator = enumerator;
            }

            public void Dispose()
            {
                internalEnumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (!internalEnumerator.MoveNext())
                    return false;

                Current = internalEnumerator.Current.Value;

                return true;
            }

            public void Reset()
            {
                internalEnumerator.Reset();
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public TSource Current
            {
                get;
                private set;
            }
        }
    }
}
