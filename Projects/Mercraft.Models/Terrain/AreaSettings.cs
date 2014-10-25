using System.Collections.Generic;
using Mercraft.Core;

namespace Mercraft.Models.Terrain
{
    /// <summary>
    ///     Defines area settings.
    /// </summary>
    public class AreaSettings
    {
        /// <summary>
        ///     Default detail index.
        /// </summary>
        public const int DefaultDetailIndex = -1;

        /// <summary>
        ///     ZIndex.
        /// </summary>
        public float ZIndex;

        /// <summary>
        ///     Splat index.
        /// </summary>
        public int SplatIndex;

        /// <summary>
        ///     Detail index.
        /// </summary>
        public int DetailIndex = DefaultDetailIndex;

        /// <summary>
        ///     Elevation.
        /// </summary>
        public float Elevation;

        /// <summary>
        ///     Gets or sets area map points.
        /// </summary>
        public List<MapPoint> Points { get; set; }
    }
}