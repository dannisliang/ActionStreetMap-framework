using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Core.World;
using Mercraft.Maps.Osm.Extensions;

namespace Mercraft.Maps.Osm.Helpers
{
    /// <summary>
    ///     Extracts Address from OSM tag collection
    /// </summary>
    public class AddressExtractor
    {
        #region Sorted lists of possible tags for Address's fields

        private static readonly List<string> NameKeyList = new List<string>
        {
            "addr:housenumber",
            "addr:housename"
        };

        private static readonly List<string> StreetKeyList = new List<string>
        {
            "addr:street",
        };

        private static readonly List<string> CodeKeyList = new List<string>
        {
            "addr:postcode",
        };

        #endregion

        public static Address Extract(IList<KeyValuePair<string, string>> tags)
        {
            return new Address
            {
                Name = GetValue(NameKeyList, tags),
                Street = GetValue(StreetKeyList, tags),
                Code = GetValue(CodeKeyList, tags)
            };
        }

        private static string GetValue(IEnumerable<string> keyList, IList<KeyValuePair<string, string>> tags)
        {
            string value;
            foreach (var key in keyList)
            {
                if (tags.TryGetValue(key, out value))
                    return value;
            }
            return null;
        }
    }
}