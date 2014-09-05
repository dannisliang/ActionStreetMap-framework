using System.Linq;
using Mercraft.Models.Utils.Geometry;

namespace Mercraft.Models.Terrain
{
    /// <summary>
    ///     Generates and fills alphamap of TerrainData using TerrainSettings provided
    /// </summary>
    public class AlphaMapGenerator
    {
        public float[, ,] GetAlphaMap(TerrainSettings settings, TerrainElement[] elements)
        {
            var layers = settings.TextureParams.Count;

            var map = new float[settings.AlphaMapSize, settings.AlphaMapSize, layers];

            // set default value
            // TODO Performance optimization: do this during scanline logic?
            for (int x = 0; x < settings.AlphaMapSize; x++)
                for (int y = 0; y < settings.AlphaMapSize; y++)
                    map[x, y, 0] = 1;

            var polygons = elements.Select(e => new Polygon(e.Points)).ToArray();

            for (int i = 0; i < polygons.Length; i++)
            {
                var index = i;
                TerrainScanLine.ScanAndFill(polygons[index], settings.AlphaMapSize,
                    (line, start, end) => Fill(map, line, start, end, elements[index].SplatIndex));
            }

            return map;
        }

        private static void Fill(float[, ,] map, int line, int start, int end, int splatIndex)
        {
            // TODO improve fill logic
            for (int i = start; i <= end; i++)
            {
                map[i, line, splatIndex] = 0.5f;
            }
        }       
    }
}