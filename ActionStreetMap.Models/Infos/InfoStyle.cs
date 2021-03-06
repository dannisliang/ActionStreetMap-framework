﻿using ActionStreetMap.Models.Geometry.Primitives;
using ActionStreetMap.Models.Geometry;

namespace ActionStreetMap.Models.Infos
{
    /// <summary>
    ///     Defines style for information node.
    /// </summary>
    public class InfoStyle
    {
        /// <summary>
        ///     Gets or sets path to material
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///     Gets or sets Uv map
        /// </summary>
        public Rect UvMap { get; set; }
    }
}
