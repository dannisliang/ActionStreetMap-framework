using System.Collections.Generic;
using System.Linq;

using Mercraft.Models.Utils.Geometry;

namespace Mercraft.Models.Terrain
{
    /// <summary>
    ///     Fills alphamap and detail maps of TerrainData using TerrainSettings provided
    /// </summary>
    public class AreaBuilder
    {
        public void Build(TerrainSettings settings, TerrainElement[] elements, float[, ,] splatMap, List<int[,]> detailMapList)
        {
            // TODO Performance optimization: do this during scanline logic?
            for (int x = 0; x < settings.AlphaMapSize; x++)
                for (int y = 0; y < settings.AlphaMapSize; y++)
                    splatMap[x, y, 0] = 1;

            for (int i = 0; i < settings.DetailParams.Count; i++)
                detailMapList.Add(new int[settings.AlphaMapSize, settings.AlphaMapSize]);

            var polygons = elements.Select(e => new Polygon(e.Points)).ToArray();
            for (int i = 0; i < polygons.Length; i++)
            {
                var index = i;
                TerrainScanLine.ScanAndFill(polygons[index], settings.AlphaMapSize, (line, start, end) =>
                        Fill(splatMap, detailMapList, line, start, end, elements[index].SplatIndex, elements[index].DetailIndex));
            }
        }

        private static void Fill(float[, ,] map, List<int[,]> detailMapList,
            int line, int start, int end, int splatIndex, int detailIndex)
        {
            var detailMap = detailIndex != -1 ? detailMapList[detailIndex] : null;
            for (int i = start; i <= end; i++)
            {
                // TODO improve fill logic: which value to use for splat?
                map[i, line, splatIndex] = 0.5f;

                if (detailMap != null)
                    detailMap[i, line] = 1;
            }
        }
    }
}