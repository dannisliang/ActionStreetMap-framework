using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Models.Geometry;

namespace Mercraft.Models.Terrain
{
    /// <summary>
    ///     Custom version of Scanline algorithm to fill terrain alphamap
    /// </summary>
    public class TerrainScanLine
    {
        public static void Fill(float[, ,] map, params AlphaMapElement[] elements)
        {
            var polygons = elements.Select(e => new Polygon(e.Points)).ToArray();

            for (int i = 0; i < polygons.Length; i++)
            {
                ScanAndFill(map, polygons[i], elements[i].SplatIndex);
            }
        }

        private static void ScanAndFill(float[, ,] map, Polygon polygon, int splatIndex)
        {
            var size = map.GetLength(0);
            var points = new List<int>();

            for (int y = 0; y < size; y++)
            {
                foreach (var segment in polygon.Segments)
                {
                    if ((segment.Start.y > y && segment.End.y > y) || // above
                       (segment.Start.y < y && segment.End.y < y)) // below
                        continue;

                    var start = segment.Start.x < segment.End.x ? segment.Start : segment.End;
                    var end = segment.Start.x < segment.End.x ? segment.End : segment.Start;

                    var x1 = start.x;
                    var y1 = start.y;
                    var x2 = end.x;
                    var y2 = end.y;

                    var d = Math.Abs(y2 - y1);

                    if (Math.Abs(d) < float.Epsilon)
                        continue;

                    // algorithm is based on fact that scan line is parallel to x-axis 
                    // so we calculate tangens of Beta angle, length of b-cathetus and 
                    // use length to get x of intersection point

                    float tanBeta = Math.Abs(x1 - x2) / d;

                    var b = Math.Abs(y1 - y);
                    var length = b * tanBeta;

                    var x = (int)(x1 + Math.Floor(length));

                    if (x >= size) x = size - 1;
                    if (x < 0) x = 0;

                    points.Add(x);
                }

                if (points.Count > 1)
                {
                    // TODO use optimized data structure
                    points.Sort();
                    points = points.Distinct().ToList();
                }

                // ignore single point
                if (points.Count == 1)
                    continue;


                if (points.Count % 2 != 0)
                {
                    // special case when two segments are parallel and they have different height
                    if (points.Count == 3)
                        points.RemoveAt(1);
                    else
                        throw new InvalidOperationException(
                            "Bug in algorithm! We're expecting to have even number of intersection points: (points.Count % 2 != 0)");
                }

                for (int i = 0; i < points.Count; i += 2)
                {
                    // TODO limit start/end poing by map size
                    Fill(map, y, points[i], points[i + 1], 1);
                }

                points.Clear();
            }
        }

        private static void Fill(float[, ,] map, int line, int start, int end, int splatIndex)
        {
            for (int i = start; i <= end; i++)
            {
                map[i, line, splatIndex] = 0.5f;
            }
        }
    }
}
