using System.Collections.Generic;

namespace Mercraft.Models.Scene
{
    /// <summary>
    /// Represents building
    /// see see http://wiki.openstreetmap.org/wiki/Key:building
    /// </summary>
    public class Building
    {
        /// <summary>
        ///  Contains copy tags from OSM layer. For development purposes only:
        /// all necessary tag info (e.g. color, address) should be processed and applied to scene
        /// </summary>
        public ICollection<KeyValuePair<string, string>> Tags { get; set; }

        public ICollection<MapPoint> Points { get; set; }
    }
}
