
using Mercraft.Core;
using Mercraft.Core.Elevation;
using Mercraft.Infrastructure.Dependencies;

namespace Mercraft.Models.Terrain
{
    public interface IHeightMapProvider
    {
        /// <summary>
        ///     Returns heightmap array for given center with given resolution
        /// </summary>
        HeightMap GetHeightMap(GeoCoordinate center, int resolution, float tileSize);

        /// <summary>
        ///     Returns corresponding height for given point from given heightmap
        /// </summary>
        float LookupHeight(HeightMap heightMap, GeoCoordinate coordinate);
    }

    public class HeightMapProvider: IHeightMapProvider
    {
        private const int MaxHeight = 8000;

        private readonly IElevationProvider _elevationProvider;

        [Dependency]
        public HeightMapProvider(IElevationProvider elevationProvider)
        {
            _elevationProvider = elevationProvider;
        }

        public HeightMap GetHeightMap(GeoCoordinate center, int resolution, float tileSize)
        {
            var map = new float[resolution, resolution];

            var bbox = BoundingBox.CreateBoundingBox(center, tileSize / 2);

            var latStep = (bbox.MaxPoint.Latitude - bbox.MinPoint.Latitude) / resolution;
            var lonStep = (bbox.MaxPoint.Longitude - bbox.MinPoint.Longitude) / resolution;

            float maxElevation = 0;

            for (int i = 0; i < resolution; i++)
            {
                var lat = bbox.MinPoint.Latitude + latStep / 2 + i * latStep;
                var lon = bbox.MinPoint.Longitude + lonStep / 2;

                for (int j = 0; j < resolution; j++)
                {
                    var elevation = _elevationProvider.GetElevation(lat, lon);

                    // TODO refactor: constant is SRTM specific which means there is no data
                    if (elevation > maxElevation && elevation < MaxHeight)
                        maxElevation = elevation;

                    map[i, j] = elevation > MaxHeight ? maxElevation : elevation;

                    lon += lonStep;
                }
            }

            // normalize
            for (int i = 0; i < resolution; i++)
              for (int j = 0; j < resolution; j++)
                map[i, j] /= maxElevation;
                
            // TODO which value to use?
            map = GenerateSmoothNoise(map, 4);

            return new HeightMap()
            {
                Center = center,
                Map = map,
                MaxElevation = maxElevation,
                Resolution = resolution,
                Size = tileSize
            };
        }

        public float LookupHeight(HeightMap heightMap, GeoCoordinate coordinate)
        {
            throw new System.NotImplementedException();
        }

        #region Smooth noise

        private static float[,] GenerateSmoothNoise(float[,] baseNoise, int octave)
        {
            int width = baseNoise.GetLength(0);
            int height = baseNoise.GetLength(1);

            float[,] smoothNoise = new float[width, height];

            int samplePeriod = 1 << octave; // calculates 2 ^ k
            float sampleFrequency = 1.0f / samplePeriod;

            for (int i = 0; i < width; i++)
            {
                //calculate the horizontal sampling indices
                int sampleI0 = (i / samplePeriod) * samplePeriod;
                int sampleI1 = (sampleI0 + samplePeriod) % width; //wrap around
                float horizontalBlend = (i - sampleI0) * sampleFrequency;

                for (int j = 0; j < height; j++)
                {
                    //calculate the vertical sampling indices
                    int sampleJ0 = (j / samplePeriod) * samplePeriod;
                    int sampleJ1 = (sampleJ0 + samplePeriod) % height; //wrap around
                    float verticalBlend = (j - sampleJ0) * sampleFrequency;

                    //blend the top two corners
                    float top = Interpolate(baseNoise[sampleI0, sampleJ0],
                        baseNoise[sampleI1, sampleJ0], horizontalBlend);

                    //blend the bottom two corners
                    float bottom = Interpolate(baseNoise[sampleI0, sampleJ1],
                        baseNoise[sampleI1, sampleJ1], horizontalBlend);

                    //final blend
                    smoothNoise[i, j] = Interpolate(top, bottom, verticalBlend);
                }
            }

            return smoothNoise;
        }

        private static float Interpolate(float x0, float x1, float alpha)
        {
            return x0 * (1 - alpha) + alpha * x1;
        }

        #endregion
    }
}
