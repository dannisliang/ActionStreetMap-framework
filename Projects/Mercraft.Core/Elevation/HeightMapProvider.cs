using System;
using ActionStreetMap.Core.Scene.Models;
using ActionStreetMap.Infrastructure.Config;
using ActionStreetMap.Infrastructure.Dependencies;

namespace ActionStreetMap.Core.Elevation
{
    /// <summary>
    ///     Defines behavior of heightmap provider.
    /// </summary>
    public interface IHeightMapProvider
    {
        /// <summary>
        ///     Returns heightmap array for given center with given resolution/
        /// </summary>
        HeightMap Get(Tile tile, int resolution);

        /// <summary>
        ///     Store heightmap in object pool to reuse in next call.
        /// </summary>
        /// <param name="heightMap">Heightmap.</param>
        void Store(HeightMap heightMap);
    }

    /// <summary>
    ///     Default realization of heightmap provider.
    /// </summary>
    public class HeightMapProvider: IHeightMapProvider, IConfigurable
    {
        private const int MaxHeight = 8000;

        private readonly IElevationProvider _elevationProvider;

        private bool _isFlat = false;
        private float[,] _map;
        private float[,] _smoothNoiseBuffer;

        internal bool DoSmooth { get; set; }

        /// <summary>
        ///     Creates HeightMapProvider.
        /// </summary>
        /// <param name="elevationProvider">Elevation provider.</param>
        [Dependency]
        public HeightMapProvider(IElevationProvider elevationProvider)
        {
            _elevationProvider = elevationProvider;
            DoSmooth = true;
        }

        /// <inheritdoc />
        public HeightMap Get(Tile tile, int resolution)
        {
            // NOTE so far we do not expect resolution change without restarting app
            if (_map == null)
                _map = new float[resolution, resolution];

            var bbox = tile.BoundingBox;

            float maxElevation = 0;
            float minElevation = 0;

            // resolve height
            if (!_isFlat)
            {
                minElevation = MaxHeight;
                var latStep = (bbox.MaxPoint.Latitude - bbox.MinPoint.Latitude) / resolution;
                var lonStep = (bbox.MaxPoint.Longitude - bbox.MinPoint.Longitude) / resolution;

                // NOTE Assume that [0,0] is bottom left corner
                var lat = bbox.MinPoint.Latitude + latStep/2;
                for (int j = 0; j < resolution; j++)
                {
                    var lon = bbox.MinPoint.Longitude + lonStep/2;
                    for (int i = 0; i < resolution; i++)
                    {
                        var elevation = _elevationProvider.GetElevation(lat, lon);

                        if (elevation > maxElevation && elevation < MaxHeight)
                            maxElevation = elevation;
                        else if (elevation < minElevation)
                            minElevation = elevation;

                        _map[j, i] = elevation > MaxHeight ? maxElevation : elevation;

                        lon += lonStep;
                    }
                    lat += latStep;
                }
                // TODO which value to use?
                if (DoSmooth)
                    _map = GenerateSmoothNoise(_map, 8);
            }

            return new HeightMap
            {
                LeftBottomCorner = tile.BottomLeft,
                RightUpperCorner = tile.TopRight,
                AxisOffset = tile.Size / resolution,
                IsFlat = false,
                Size = tile.Size,
                Data = _map,
                MinElevation = minElevation,
                MaxElevation = maxElevation,
                Resolution = resolution,
            };
        }

        /// <inheritdoc />
        public void Store(HeightMap heightMap)
        {
            Array.Clear(_map, 0, _map.Length);
        }

        /// <inheritdoc />
        public void Configure(IConfigSection configSection)
        {
            _isFlat = configSection.GetBool("flat", false);
        }

        #region Smooth noise

        // TODO I don't like current smooth algorithm

        private float[,] GenerateSmoothNoise(float[,] baseNoise, int octave)
        {
            int width = baseNoise.GetLength(0);
            int height = baseNoise.GetLength(1);
            if (_smoothNoiseBuffer == null)
                _smoothNoiseBuffer = new float[width, height];       

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
                    _smoothNoiseBuffer[i, j] = Interpolate(top, bottom, verticalBlend);
                }
            }

            return _smoothNoiseBuffer;
        }

        private static float Interpolate(float x0, float x1, float alpha)
        {
            return x0 * (1 - alpha) + alpha * x1;
        }

        #endregion
    }
}
