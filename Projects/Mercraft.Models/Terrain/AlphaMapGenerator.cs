using System.Collections.Generic;
using System.Linq;
using Mercraft.Models.Terrain.Areas;
using Mercraft.Models.Terrain.Roads;
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

        private TerrainData _terrainData;
        private AlphaMapElement[] _polygons;

        public AlphaMapGenerator(TerrainSettings settings)
        {
            _settings = settings;
            _terrainPosition = _settings.CornerPosition;
        }

        public void FillAlphaMap(TerrainData terrainData)
        {
            _terrainData = terrainData;
            var layers = _settings.SplatPrototypes.Length;

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

            terrainData.alphamapResolution = _settings.AlphaMapSize;
            terrainData.SetAlphamaps(0, 0, map);
        }

        private void CreatePolygons()
        {
            var widthRatio = _settings.AlphaMapSize/_terrainData.size.x;
            var heightRatio = _settings.AlphaMapSize/_terrainData.size.z;

            var areaBuilder = new AreaBuilder(_terrainPosition, widthRatio, heightRatio);
            var roadBuilder = new RoadBuilder(_terrainPosition, widthRatio, heightRatio);

            var areas = areaBuilder.Build(_settings.Areas);
            var roads = roadBuilder.Build(_settings.Roads);

            var tempPolygons = new List<AlphaMapElement>(areas.Length + roads.Length);
            tempPolygons.AddRange(areas);
            tempPolygons.AddRange(roads);

            _polygons = tempPolygons.OrderBy(p => p.SplatIndex).ToArray();
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