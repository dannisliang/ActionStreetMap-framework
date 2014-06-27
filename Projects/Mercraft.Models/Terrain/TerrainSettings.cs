using Mercraft.Models.Terrain.Areas;
using Mercraft.Models.Terrain.Roads;
using UnityEngine;

namespace Mercraft.Models.Terrain
{
    public class TerrainSettings
    {
        /// <summary>
        /// This is the control map that controls how the splat textures will be blended
        /// </summary>
        public int AlphaMapSize = 512;
        public float TerrainSize = 15;

        /// <summary>
        /// Higher number will create more detailed height maps
        /// </summary>
        public int HeightMapSize = 513;
        public int TerrainHeight = 1;

        //Noise settings. A higher frq will create larger scale details. Each seed value will create a unique look
        public int GroundSeed = 0;
        public float GroundFrq = 800.0f;
        public int MountainSeed = 1;
        public float MountainFrq = 1200.0f;

        /// <summary>
        /// A lower pixel error will draw terrain at a higher Level of detail but will be slower
        /// </summary>
        public float PixelMapError = 6.0f;

        /// <summary>
        /// The distance at which the low res base map will be drawn. Decrease to increase performance
        /// </summary>
        public float BaseMapDist = 1000.0f;

        public SplatPrototype[] SplatPrototypes;

        public Road[] Roads;
        public Area[] Areas;

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
