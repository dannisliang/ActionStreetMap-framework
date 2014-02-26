using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercraft.Models.Map
{
    public class TileSettings
    {
        /// <summary>
        /// Stores geo coordinate for (0,0)
        /// NOTE: possible collisions due to projection precision error
        /// </summary>
        public GeoCoordinate RelativeNullPoint { get; set; }

        /// <summary>
        /// Stores size of tile (Unity metric)
        /// </summary>
        public float Size { get; set; }
    }
}
