using System.Collections.Generic;
using System.Linq;

namespace Mercraft.Core.Utilities
{
    public static class Extensions
    {
        public static bool AreSame(this MapPoint point1, MapPoint point2)
        {
            return MathUtility.AreEqual(point1.X, point2.X) && MathUtility.AreEqual(point1.Y, point2.Y);
        }

        public static bool ContainsKeyValue(this IList<KeyValuePair<string, string>> collection, string key,
            string value)
        {
            return collection != null && collection.Any(keyValuePair => keyValuePair.Key == key && keyValuePair.Value == value);
        }

        public static bool ContainsKey(this IList<KeyValuePair<string, string>> collection, string key)
        {
            return collection != null && collection.Any(keyValuePair => keyValuePair.Key == key);
        }

        public static bool IsNotEqual(this IList<KeyValuePair<string, string>> collection, string key,
            string value)
        {
            return collection.All(keyValuePair => keyValuePair.Key != key || keyValuePair.Value == value);
        }

        public static bool IsLess(this IList<KeyValuePair<string, string>> collection, string key,
            string value)
        {
            return CompareValues(collection, key, value, false);
        }

        public static bool IsGreater(this IList<KeyValuePair<string, string>> collection, string key,
            string value)
        {
            return CompareValues(collection, key, value, true);
        }

        /// <summary>
        /// Compares value in collection
        /// </summary>
        private static bool CompareValues(IList<KeyValuePair<string, string>> collection, string key,
            string value, bool isGreater)
        {
            float target = float.Parse(value);
            for (int i = 0; i < collection.Count; i++)
            {
                if (collection[i].Key != key)
                    continue;

                float fValue = 0;
                if (float.TryParse(collection[i].Value, out fValue) &&
                    (isGreater? fValue > target: fValue < target))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
