using System.Collections.Generic;

namespace Mercraft.Core.Utilities
{
    public static class Extensions
    {
        public static bool AreSame(this MapPoint point1, MapPoint point2)
        {
            return MathUtility.AreEqual(point1.X, point2.X) && MathUtility.AreEqual(point1.Y, point2.Y);
        }

        public static bool ContainsKeyValue(this Dictionary<string, string> collection, string key,
            string value)
        {
            return collection != null && collection.ContainsKey(key) && collection[key] == value;
        }

        public static bool ContainsKey(this Dictionary<string, string> collection, string key)
        {
            return collection != null && collection.ContainsKey(key);
        }

        public static bool NotContainsKey(this Dictionary<string, string> collection, string key)
        {
            // NOTE should we consider null collection as valid case for this?
            return collection != null && !collection.ContainsKey(key);
        }

        public static bool IsNotEqual(this Dictionary<string, string> collection, string key, string value)
        {
            return collection != null && collection.ContainsKey(key) && collection[key] != value;
        }

        public static bool IsLess(this Dictionary<string, string> collection, string key,
            string value)
        {
            return collection != null && CompareValues(collection, key, value, false);
        }

        public static bool IsGreater(this Dictionary<string, string> collection, string key,
            string value)
        {
            return collection != null && CompareValues(collection, key, value, true);
        }

        /// <summary>
        ///     Compares value in collection
        /// </summary>
        private static bool CompareValues(Dictionary<string, string> collection, string key,
            string value, bool isGreater)
        {
            if (!collection.ContainsKey(key))
                return false;

            float target = float.Parse(value);
            var item = collection[key];
            float fValue = 0;
            return float.TryParse(item, out fValue) && (isGreater ? fValue > target : fValue < target);
        }
    }
}
