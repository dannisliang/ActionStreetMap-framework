using System.Collections.Generic;
using System.Linq;
using Mercraft.Models.Terrain;
using UnityEngine;

namespace Mercraft.Models.Areas
{
    public class AreaBuilder
    {
        private readonly Vector2 _terrainPosition;
        private readonly float _widthRatio;
        private readonly float _heightRatio;

        public AreaBuilder(Vector2 terrainPosition, float widthRatio, float heightRatio)
        {
            _terrainPosition = terrainPosition;
            _widthRatio = widthRatio;
            _heightRatio = heightRatio;
        }

        public AlphaMapElement[] Build(List<AreaSettings> areas)
        {
            return areas.Select(a => new AlphaMapElement()
            {
                ZIndex = a.ZIndex,
                SplatIndex = a.SplatIndex,
                Points = a.Points.Select(p => 
                    TerrainUtils.ConvertWorldToTerrain(p, _terrainPosition, _widthRatio, _heightRatio)).ToArray()
            }).ToArray();
        }
    }
}
