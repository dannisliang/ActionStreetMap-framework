using System.Collections.Generic;
using System.Linq;
using Mercraft.Models.Areas;
using Mercraft.Models.Unity;
using UnityEngine;

namespace Mercraft.Models.Terrain
{
    /// <summary>
    ///     Generates and fills alphamap of TerrainData using TerrainSettings provided
    /// </summary>
    public class AlphaMapGenerator
    {
        private readonly TerrainSettings _settings;
        private readonly Vector2 _terrainPosition;

        private ITerrainData _terrainData;
        private AlphaMapElement[] _polygons;

        public AlphaMapGenerator(TerrainSettings settings)
        {
            _settings = settings;
            _terrainPosition = _settings.CornerPosition;
        }

        public float[,,] GetAlphaMap(ITerrainData terrainData)
        {
            _terrainData = terrainData;
            var layers = _settings.TextureParams.Count;

            CreatePolygons();

            float[,,] map = new float[_settings.AlphaMapSize, _settings.AlphaMapSize, layers];

            for (int x = 0; x < _settings.AlphaMapSize; x++)
            {
                for (int y = 0; y < _settings.AlphaMapSize; y++)
                {
                    var splatIndecies = GetPolygonMainSplatIndex(new Vector2(x, y)).ToArray();
                    // TODO process different layers with blending
                    // set default tecture
                    if (splatIndecies.Length == 0)
                    {
                        map[x, y, 0] = 1;
                    }
                    else
                    {
                        foreach (var splatIndex in splatIndecies)
                        {
                            map[x, y, splatIndex] = 1;
                        }
                    }
                }
            }

            terrainData.AlphamapResolution = _settings.AlphaMapSize;
            return map;
        }

        private void CreatePolygons()
        {
            var widthRatio = _settings.AlphaMapSize/_terrainData.Size.x;
            var heightRatio = _settings.AlphaMapSize/_terrainData.Size.z;

            var areaBuilder = new AreaBuilder(_terrainPosition, widthRatio, heightRatio);
            var areas = areaBuilder.Build(_settings.Areas);
            _polygons = areas.OrderBy(p => p.SplatIndex).ToArray();
        }

        private IEnumerable<int> GetPolygonMainSplatIndex(Vector2 point)
        {
            for (int i = 0; i < _polygons.Length; i++)
            {
                if (TerrainUtils.IsPointInPolygon(_polygons[i].Points, point))
                {
                    yield return _polygons[i].SplatIndex;
                }
            }
        }
    }
}