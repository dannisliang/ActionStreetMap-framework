using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mercraft.Core.Utilities
{
    public static class Extensions
    {
        public static bool AreSame(this Vector2 point1, Vector2 point2)
        {
            return MathUtility.AreEqual(point1.x, point2.x) && MathUtility.AreEqual(point1.y, point2.y);
        }

        public static bool ContainsKeyValue(this ICollection<KeyValuePair<string, string>> collection, string key,
            string value)
        {
            return collection != null && collection.Any(keyValuePair => keyValuePair.Key == key && keyValuePair.Value == value);
        }

        public static bool ContainsKey(this ICollection<KeyValuePair<string, string>> collection, string key)
        {
            return collection != null && collection.Any(keyValuePair => keyValuePair.Key == key);
        }
    }
}
