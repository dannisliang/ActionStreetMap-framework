using System;
using System.Collections.Generic;

namespace Mercraft.Infrastructure.Extenstions
{
    public static class LinqExtenstions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> knownKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<TSource> DistinctByLast<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var map = new Dictionary<TKey, TSource>();
            foreach (TSource element in source)
            {
                map[keySelector(element)] = element;
            }
            return map.Values;
        }
    }
}
