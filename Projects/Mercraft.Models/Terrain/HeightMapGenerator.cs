using System.Linq;
using Mercraft.Models.Geometry;
using UnityEngine;

namespace Mercraft.Models.Terrain
{
    public class HeightMapGenerator
    {
        public float[,] FillHeights(TerrainSettings settings, TerrainElement[] elements)
        {
            var map = new float[settings.HeightMapSize, settings.HeightMapSize];
            
            for (int x = 0; x < settings.HeightMapSize; x++)
                for (int y = 0; y < settings.HeightMapSize; y++)
                    map[y, x] = 1;

            var polygons = elements.Select(e => new Polygon(e.Points)).ToArray();

            for (int i = 0; i < polygons.Length; i++)
            {
                var index = i;

                Debug.Log("polygon" + i);
                foreach (var vertex in polygons[i].Verticies)
                {
                    Debug.Log(vertex);
                }

                TerrainScanLine.ScanAndFill(polygons[index], settings.HeightMapSize,
                    (line, start, end) => Fill(map, line, start, end, elements[index].ZIndex));
            }

            return map;
        }

        private static void Fill(float[,] map, int line, int start, int end, float zIndex)
        {
            for (int i = start; i <= end; i++)
            {
               map[i, line] = 0.5f;
            }
        }
    }
}
