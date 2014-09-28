using System;
using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Core.Elevation;
using Mercraft.Models.Geometry;

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

        // reusable buffer for lines
        private MapPoint[] _mapPointBuffer = new MapPoint[4];

        public void Recycle(HeightMap heightMap)
        {
            _heightMap = heightMap;
            _data = _heightMap.Data;
            _ratio = heightMap.Size / heightMap.Resolution;
            _size = heightMap.Resolution;
            _lastIndex = _size - 1;
        }

        public void Clear()
        {
            _heightMap = null;
            _data = null;
        }

        public void AdjustLine(MapPoint start, MapPoint end, float width)
        {
            SetOffsetPoints(start, end, width);

            var elevation = start.Elevation < end.Elevation ? start.Elevation : end.Elevation;

            // TODO this is not good  idea to create instance of anonymous class for every small road chunk
            SimpleScanLine.Fill(_mapPointBuffer, _size, (scanline, s, e) =>
               Fill(scanline, s, e, elevation));
        }

        public void AdjustPolygon(MapPoint[] points, float elevation)
        {
            var mapPointBuffer = new MapPoint[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                var point = points[i];
                mapPointBuffer[i] = GetHeightMapPoint(point.X, point.Y);
            }

            ScanLine.FillPolygon(mapPointBuffer, (scanline, s, e) => 
               Fill(scanline, s, e, elevation));
        }

        private void Fill(int line, int start, int end, float elevation)
        {
            if ((start > _lastIndex) || (end < 0) || line < 0 || line > _lastIndex)
                return;

            var s = start > _lastIndex ? _lastIndex : start;
            s = s < 0 ? 0 : s;

            var e = end > _lastIndex ? _lastIndex : end;

            for (int i = s; i <= e && i < _size; i++)
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
