using System.Collections.Generic;

namespace Mercraft.Core.Utilities
{
    internal static class TagExtensions
    {
        public static bool ContainsKeyValue(this Dictionary<string, string> collection, string key,
            string value)
        {
            string actualValue;
            return collection != null && collection.TryGetValue(key, out actualValue) && actualValue == value;
        }

        public static bool IsNotEqual(this Dictionary<string, string> collection, string key, string value)
        {
            string actualValue;
            return collection != null && collection.TryGetValue(key, out actualValue) && actualValue != value;
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
            string actualValue;
            if (!collection.TryGetValue(key, out actualValue))
                return false;

            float target = float.Parse(value);
            float fValue;
            return float.TryParse(actualValue, out fValue) && (isGreater ? fValue > target : fValue < target);
        }
    }
}
