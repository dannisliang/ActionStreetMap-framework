using System;
using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Core.Elevation;

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
        private float[,] _data;
        private float _ratio;

        private int _scanLineStart;
        private int _scanLineEnd;

        // reusable buffers
        private List<int> _scanListBuffer = new List<int>(2);
        private MapPoint[] _mapPointBuffer = new MapPoint[4];

        public void Recycle(HeightMap heightMap)
        {
            _heightMap = heightMap;
            _data = _heightMap.Data;
            _ratio = heightMap.Size / heightMap.Resolution;
            _size = heightMap.Resolution;
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

            for (int i = 0; i < points.Length; i++)
            {
                var point = points[i];
                _mapPointBuffer[i] = GetHeightMapPoint(point.X, point.Y);
            }

            InitializeScanLine();
            ScanAndFill(elevation);
        }

        private void InitializeScanLine()
        {
            _scanLineStart = int.MaxValue;
            _scanLineEnd = int.MinValue;

            for (int i = 0; i < _mapPointBuffer.Length; i++)
            {
                var point = _mapPointBuffer[i];
                if (_scanLineEnd < point.Y && point.Y >= 0)
                    _scanLineEnd = (int)point.Y;
                if (_scanLineStart > point.Y && point.Y >= 0)
                    _scanLineStart = (int)point.Y;
            }

            _scanLineStart = _scanLineStart < _size ? _scanLineStart : _size - 1;
            _scanLineEnd = _scanLineEnd < _size ? _scanLineEnd : _size - 1;
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

                    float tanBeta = Math.Abs(x1 - x2) / d;

                    var b = Math.Abs(y1 - z);
                    var length = b * tanBeta;

                    var x = (int)(x1 + Math.Floor(length));

                    if (x >= _size) x = _size - 1;
                    if (x < 0) x = 0;

                    _scanListBuffer.Add(x);
                }

                if (_scanListBuffer.Count > 1)
                {
                    _scanListBuffer.Sort();
                    Fill(z, _scanListBuffer[0], _scanListBuffer[_scanListBuffer.Count - 1], elevation);
                }

                _scanListBuffer.Clear();
            }
        }

        private void Fill(int line, int start, int end, float elevation)
        {
            for (int i = start; i <= end && i < _size; i++)
            {
                _data[line, i] = elevation;
            }
        }

        private MapPoint GetHeightMapPoint(float x, float y)
        {
            return new MapPoint()
            {
                X = (int) Math.Ceiling((x - _heightMap.LeftBottomCorner.X)/_ratio),
                Y = (int) Math.Ceiling(((y - _heightMap.LeftBottomCorner.Y)/_ratio))
            };
        }

        private void SetOffsetPoints(MapPoint point1, MapPoint point2, float offset)
        {
            float x1 = point1.X, x2 = point2.X, z1 = point1.Y, z2 = point2.Y;
            float l = (float)Math.Sqrt((x1 - x2) * (x1 - x2) + (z1 - z2) * (z1 - z2));

            var zOffset = (z2 - z1)/l;
            var xOffset = (x1 - x2)/l;

            _mapPointBuffer[3] = GetHeightMapPoint(x1 + offset * zOffset, z1 + offset * xOffset);
            _mapPointBuffer[2] = GetHeightMapPoint(x2 + offset * zOffset, z2 + offset * xOffset);
            
            _mapPointBuffer[1] = GetHeightMapPoint(x2 - offset * zOffset, z2 - offset * xOffset);
            _mapPointBuffer[0] = GetHeightMapPoint(x1 - offset * zOffset, z1 - offset * xOffset);
        }
    }
}
