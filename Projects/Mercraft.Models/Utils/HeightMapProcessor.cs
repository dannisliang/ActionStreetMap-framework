using System;
using Mercraft.Core;
using Mercraft.Core.Elevation;
using Mercraft.Infrastructure.Primitives;

namespace Mercraft.Models.Utils
{
    /// <summary>
    ///     Provides logic to adjust height of terrain.
    ///     NOTE this class has performance critical impact on rendering time if elevation is enabled. 
    ///     So readability was sacrified in favor of performance
    ///     NOT thread safe! 
    /// </summary>
    public class HeightMapProcessor
    {
        private HeightMap _heightMap;
        private int _size;
        private int _lastIndex;
        private float[,] _data;
        private float _ratio;

        private int _scanLineStart;
        private int _scanLineEnd;

        // reusable buffers
        private SortedSet<int> _scanListBuffer = new SortedSet<int>();

        private MapPoint[] _mapPointBuffer = new MapPoint[4];

        private bool _outOfTile;

        public void Recycle(HeightMap heightMap)
        {
            _heightMap = heightMap;
            _data = _heightMap.Data;
            _ratio = heightMap.Size / heightMap.Resolution;
            _size = heightMap.Resolution;
            _lastIndex = _size - 1;
        }

        public void AdjustLine(MapPoint start, MapPoint end, float width)
        {
            SetOffsetPoints(start, end, width);

            var elevation = start.Elevation < end.Elevation ? start.Elevation : end.Elevation;

            InitializeScanLine();
            ScanAndFill(elevation);
        }

        public void AdjustPolygon(MapPoint[] points, float elevation)
        {
            _mapPointBuffer = new MapPoint[points.Length];

            for (int i = 0; i < _mapPointBuffer.Length; i++)
            {
                var point = points[i];
                _mapPointBuffer[i] = GetHeightMapPoint(point.X, point.Y);
            }

            InitializeScanLine();

            if (!_outOfTile)
                ScanAndFill(elevation);
        }

        private void InitializeScanLine()
        {
            _outOfTile = true;
            _scanLineStart = int.MaxValue;
            _scanLineEnd = int.MinValue;

            // search start and end values
            for (int i = 0; i < _mapPointBuffer.Length; i++)
            {
                var point = _mapPointBuffer[i];

                if (point.Y <= 0 || point.Y >= _size)
                    continue;

                _outOfTile = false;
                if (_scanLineEnd < point.Y)
                    _scanLineEnd = (int)point.Y;
                if (_scanLineStart > point.Y)
                    _scanLineStart = (int)point.Y;
            }
        }

        /// <summary>
        ///     This is optimized version for particular case of algorithm for terrain
        /// </summary>
        private void ScanAndFill(float elevation)
        {
            for (int z = _scanLineStart; z <= _scanLineEnd; z++)
            {
                for (int i = 0; i < _mapPointBuffer.Length; i++)
                {
                    var currentIndex = i;
                    var nextIndex = i == _mapPointBuffer.Length - 1 ? 0 : i + 1;

                    if ((_mapPointBuffer[currentIndex].Y > z && _mapPointBuffer[nextIndex].Y > z) || // above
                      (_mapPointBuffer[currentIndex].Y < z && _mapPointBuffer[nextIndex].Y < z)) // below
                        continue;

                    var start = _mapPointBuffer[currentIndex].X < _mapPointBuffer[nextIndex].X ?
                          _mapPointBuffer[currentIndex]
                        : _mapPointBuffer[nextIndex];

                    var end = _mapPointBuffer[currentIndex].X < _mapPointBuffer[nextIndex].X
                        ? _mapPointBuffer[nextIndex]
                        : _mapPointBuffer[currentIndex];

                    var x1 = start.X;
                    var y1 = start.Y;
                    var x2 = end.X;
                    var y2 = end.Y;

                    var d = Math.Abs(y2 - y1);

                    if (Math.Abs(d) < float.Epsilon)
                        continue;

                    double tanBeta = Math.Abs(x1 - x2) / d;

                    var b = Math.Abs(y1 - z);
                    var length = b * tanBeta;

                    var x = (int)Math.Round(x1 + Math.Floor(length));
                    _scanListBuffer.Add(x);
                }

                var count = _scanListBuffer.Values.Count;
                if (count > 1)
                {
                    for (int i = 0; i < count - 1; i++)
                    {
                        var first = _scanListBuffer.Values[i];
                        var second = _scanListBuffer.Values[++i];

                        // algorithm contains bugs for some cases
                        if (i < count - 1 && _scanListBuffer.Values[i + 1] < _lastIndex && Math.Abs(first - second) <= 2)
                            second = _scanListBuffer.Values[++i];

                        if (count % 2 != 0 && i <= count - 2 && second > _lastIndex)
                            break;

                        Fill(z, first, second, elevation);
                    }
                }

                _scanListBuffer.Clear();
            }
        }

        private void Fill(int line, int start, int end, float elevation)
        {
            if ((start > _lastIndex) || (end < 0))
                return;

            var s = start < 0 ? 0 : start;
            var e = end > _lastIndex ? _lastIndex : end;

            for (int i = s; i <= e; i++)
            {
                _data[line, i] = elevation;
            }
        }

        private MapPoint GetHeightMapPoint(float x, float y)
        {
            return new MapPoint()
            {
                X = (int)Math.Ceiling((x - _heightMap.LeftBottomCorner.X) / _ratio),
                Y = (int)Math.Ceiling(((y - _heightMap.LeftBottomCorner.Y) / _ratio))
            };
        }

        private void SetOffsetPoints(MapPoint point1, MapPoint point2, float offset)
        {
            float x1 = point1.X, x2 = point2.X, z1 = point1.Y, z2 = point2.Y;
            float l = (float)Math.Sqrt((x1 - x2) * (x1 - x2) + (z1 - z2) * (z1 - z2));

            var zOffset = (z2 - z1) / l;
            var xOffset = (x1 - x2) / l;

            _mapPointBuffer[3] = GetHeightMapPoint(x1 + offset * zOffset, z1 + offset * xOffset);
            _mapPointBuffer[2] = GetHeightMapPoint(x2 + offset * zOffset, z2 + offset * xOffset);

            _mapPointBuffer[1] = GetHeightMapPoint(x2 - offset * zOffset, z2 - offset * xOffset);
            _mapPointBuffer[0] = GetHeightMapPoint(x1 - offset * zOffset, z1 - offset * xOffset);
        }
    }
}
