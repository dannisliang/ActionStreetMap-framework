using System;
using System.Collections.Generic;
using Mercraft.Models.Utils.Geometry;

namespace Mercraft.Models.Terrain
{
    /// <summary>
    ///     Custom version of Scanline algorithm to process terrain alphamap/heightmap
    /// </summary>
    public class TerrainScanLine
    {
        public static void ScanAndFill(Polygon polygon, int size,
            Action<int, int, int> fillAction)
        {
            var points = new List<int>();
            for (int z = 0; z < size; z++)
            {
                foreach (var segment in polygon.Segments)
                {
                    if ((segment.Start.z > z && segment.End.z > z) || // above
                       (segment.Start.z < z && segment.End.z < z)) // below
                        continue;

                    var start = segment.Start.x < segment.End.x ? segment.Start : segment.End;
                    var end = segment.Start.x < segment.End.x ? segment.End : segment.Start;

                    var x1 = start.x;
                    var z1 = start.z;
                    var x2 = end.x;
                    var z2 = end.z;

                    var d = Math.Abs(z2 - z1);

                    if (Math.Abs(d) < float.Epsilon)
                        continue;

                    // algorithm is based on fact that scan line is parallel to x-axis 
                    // so we calculate tangens of Beta angle, length of b-cathetus and 
                    // use length to get x of intersection point

                    float tanBeta = Math.Abs(x1 - x2) / d;

                    var b = Math.Abs(z1 - z);
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
                    //points = points.Distinct().ToList();

                    // merge connected ranges
                    for (int i = points.Count - 1; i > 0; i--)
                    {
                        if (i != 0 && points[i] == points[i - 1])
                        {
                            points.RemoveAt(i);
                            if (points.Count % 2 != 0)
                                points.RemoveAt(--i);
                        }
                    }
                }

                // ignore single point
                if (points.Count == 1)
                    continue;


                if (points.Count % 2 != 0)
                {
                    throw new InvalidOperationException(
                        "Bug in algorithm! We're expecting to have even number of intersection points: (points.Count % 2 != 0)");
                }

                for (int i = 0; i < points.Count; i += 2)
                    fillAction(z, points[i], points[i + 1]);

                points.Clear();
            }
        }
    }
}
