using System.Linq;
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

        public AlphaMapGenerator(TerrainSettings settings)
        {
            _settings = settings;
            _terrainPosition = _settings.CornerPosition;
        }

        public float[,,] GetAlphaMap(ITerrainData terrainData)
        {
            _terrainData = terrainData;
            var layers = _settings.TextureParams.Count;

            var map = new float[_settings.AlphaMapSize, _settings.AlphaMapSize, layers];

            var polygons = CreatePolygons();

            // set default value
            // TODO Performance optimization: do this during scanline logic?
            for (int x = 0; x < _settings.AlphaMapSize; x++)
                for (int y = 0; y < _settings.AlphaMapSize; y++)
                    map[x, y, 0] = 1;          

            TerrainScanLine.Fill(map, polygons);

            terrainData.AlphamapResolution = _settings.AlphaMapSize;
            return map;
        }

        private AlphaMapElement[] CreatePolygons()
        {
            var widthRatio = _settings.AlphaMapSize/_terrainData.Size.x;
            var heightRatio = _settings.AlphaMapSize/_terrainData.Size.z;

            var areas = _settings.Areas.Select(a => new AlphaMapElement()
            {
                ZIndex = a.ZIndex,
                SplatIndex = a.SplatIndex,
                Points = a.Points.Select(p =>
                    ConvertWorldToTerrain(p.X, p.Y, _terrainPosition, widthRatio, heightRatio)).ToArray()
            }).ToArray();

            return areas.OrderBy(p => p.SplatIndex).ToArray();
        }

        private static Vector2 ConvertWorldToTerrain(float x, float y, Vector2 terrainPosition, float widthRatio, float heightRatio)
        {
            return new Vector2
            {
                // NOTE Coords are inverted here!
                y = (x - terrainPosition.x) * widthRatio,
                x = (y - terrainPosition.y) * heightRatio
            };
        } 
    }
}