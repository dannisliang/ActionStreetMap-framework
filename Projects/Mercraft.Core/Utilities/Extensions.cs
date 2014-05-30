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
    }
}
