using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Models.Terrain
{
    public class HeightMapGenerator
    {
        private readonly TerrainSettings _settings;
        public HeightMapGenerator(TerrainSettings settings)
        {
            _settings = settings;
        }

        public void FillHeights(float[,] htmap)
        {
            var groundNoise = new PerlinNoise(_settings.GroundSeed);
            var mountainNoise = new PerlinNoise(_settings.MountainSeed);

            float ratio = _settings.TerrainSize / (float)_settings.HeightMapSize;

            for (int x = 0; x < _settings.HeightMapSize; x++)
            {
                for (int y = 0; y < _settings.HeightMapSize; y++)
                {
                    float worldPosX = (x + _settings.CenterPosition.x * (_settings.HeightMapSize - 1)) * ratio;
                    float worldPosZ = (y + _settings.CenterPosition.y * (_settings.HeightMapSize - 1)) * ratio;

                    float mountains = Mathf.Max(0.0f,
                        mountainNoise.FractalNoise2D(worldPosX, worldPosZ, 6, _settings.MountainFrq, 0.8f));

                    float plain = groundNoise.FractalNoise2D(worldPosX, worldPosZ, 4, _settings.GroundFrq, 0.1f) + 0.1f;

                    htmap[y, x] = plain + mountains;
                }
            }
        }
    }
}
