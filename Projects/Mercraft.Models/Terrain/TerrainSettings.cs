using System.Collections.Generic;
using UnityEngine;

namespace Mercraft.Models.Terrain
{
    public class TerrainSettings
    {
        /// <summary>
        /// This is the control map that controls how the splat textures will be blended
        /// </summary>
        public int AlphaMapSize = 512;
        public float TerrainSize = 500;

        /// <summary>
        /// Higher number will create more detailed height maps
        /// </summary>
        public int HeightMapSize = 513;
        public float TerrainHeight = 10;

        /// <summary>
        /// A lower pixel error will draw terrain at a higher Level of detail but will be slower
        /// </summary>
        public float PixelMapError = 6.0f;

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
                return new Vector2(CenterPosition.x - TerrainSize / 2f,
                CenterPosition.y - TerrainSize / 2f);
            }
        }
    }
}
