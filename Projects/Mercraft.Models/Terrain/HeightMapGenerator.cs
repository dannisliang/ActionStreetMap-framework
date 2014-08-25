using System.Linq;
using Mercraft.Models.Geometry;

namespace Mercraft.Models.Terrain
{
    public class HeightMapGenerator
    {
        public float[,] FillHeights(TerrainSettings settings, TerrainElement[] elements)
        {
            var map = new float[settings.HeightMapSize, settings.HeightMapSize];

            var polygons = elements.Select(e => new Polygon(e.Points)).ToArray();

            for (int i = 0; i < polygons.Length; i++)
            {
                var index = i;
                TerrainScanLine.ScanAndFill(polygons[index], settings.HeightMapSize,
                    (line, start, end) => Fill(map, line, start, end));
            }

            return map;
        }

        private static void Fill(float[,] map, int line, int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                // TODO
                map[i, line] += 20;
            }
        }
    }
}
