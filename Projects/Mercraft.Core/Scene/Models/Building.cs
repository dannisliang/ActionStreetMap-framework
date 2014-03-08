using System.Collections.Generic;

namespace Mercraft.Core.Scene.Models
{
    /// <summary>
    /// Represents building
    /// see see http://wiki.openstreetmap.org/wiki/Key:building
    /// </summary>
    public class Building
    {
        public string Id { get; set; }
        /// <summary>
        ///  Contains copy tags from OSM layer. For development purposes only:
        /// all necessary tag info (e.g. color, address) should be processed and applied to scene
        /// </summary>
        public ICollection<KeyValuePair<string, string>> Tags { get; set; }

        public ICollection<GeoCoordinate> Points { get; set; }
    }
}
