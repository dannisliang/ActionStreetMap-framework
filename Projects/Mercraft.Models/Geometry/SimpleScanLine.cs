using System;
using System.Collections.Generic;
using Mercraft.Core;

namespace Mercraft.Models.Geometry
{
    public class SimpleScanLine
    {
        private static List<int> _scanListBuffer = new List<int>(2);
        /// <summary>
        ///     This is optimized version for particular case of algorithm for road segments
        /// </summary>
        public static void Fill(MapPoint[] mapPointBuffer, int size, Action<int,int,int> fillAction)
        {
            var scanLineStart = int.MaxValue;
            var scanLineEnd = int.MinValue;

            // search start and end values
            for (int i = 0; i < mapPointBuffer.Length; i++)
            {
                var point = mapPointBuffer[i];

                if (point.Y <= 0 || point.Y >= size)
                    continue;

                if (scanLineEnd < point.Y)
                    scanLineEnd = (int)point.Y;
                if (scanLineStart > point.Y)
                    scanLineStart = (int)point.Y;
            }

            for (int z = scanLineStart; z <= scanLineEnd; z++)
            {
                for (int i = 0; i < mapPointBuffer.Length; i++)
                {
                    var currentIndex = i;
                    var nextIndex = i == mapPointBuffer.Length - 1 ? 0 : i + 1;

                    if ((mapPointBuffer[currentIndex].Y > z && mapPointBuffer[nextIndex].Y > z) || // above
                      (mapPointBuffer[currentIndex].Y < z && mapPointBuffer[nextIndex].Y < z)) // below
                        continue;

                    var start = mapPointBuffer[currentIndex].X < mapPointBuffer[nextIndex].X ?
                          mapPointBuffer[currentIndex]
                        : mapPointBuffer[nextIndex];

                    var end = mapPointBuffer[currentIndex].X < mapPointBuffer[nextIndex].X
                        ? mapPointBuffer[nextIndex]
                        : mapPointBuffer[currentIndex];

                    var x1 = start.X;
                    var y1 = start.Y;
                    var x2 = end.X;
                    var y2 = end.Y;

                    var d = Math.Abs(y2 - y1);

                    if (Math.Abs(d) < float.Epsilon)
                        continue;

                    float tanBeta = Math.Abs(x1 - x2) / d;

                    var b = Math.Abs(y1 - z);
                    var length = b * tanBeta;

                    var x = (int)(x1 + Math.Floor(length));

                    if (x >= size) x = size - 1;
                    if (x < 0) x = 0;

                    _scanListBuffer.Add(x);
                }

                if (_scanListBuffer.Count > 1)
                {
                    _scanListBuffer.Sort();
                    fillAction(z, _scanListBuffer[0], _scanListBuffer[_scanListBuffer.Count - 1]);
                }

                _scanListBuffer.Clear();
            }
        }
    }
}
