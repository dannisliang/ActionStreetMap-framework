using System.Collections.Generic;
using Mercraft.Core.Elevation;
using UnityEngine;

namespace Mercraft.Models.Terrain
{
    public class TerrainSettings
    {
        /// <summary>
        /// This is the control map that controls how the splat textures will be blended
        /// </summary>
        public int AlphaMapSize = 512;
        public float Size = 500;

        public HeightMap HeightMap;

        /// <summary>
        /// A lower pixel error will draw terrain at a higher Level of detail but will be slower
        /// </summary>
        public float PixelMapError = 100f;

        /// <summary>
        /// The distance at which the low res base map will be drawn. Decrease to increase performance
        /// </summary>
        public float BaseMapDist = 200.0f;

        public List<List<string>> TextureParams;

        public List<AreaSettings> Areas;
        public List<AreaSettings> Elevations;

        public float ZIndex;
        public Vector2 CenterPosition;
        public Vector2 CornerPosition
        {
            get
            {
                return new Vector2(CenterPosition.x - Size / 2f,
                CenterPosition.y - Size / 2f);
            }
        }
    }
}
