using System;

namespace Mercraft.Core
{
    public struct MapPoint
    {
        public float X { get; set; }
        public float Y { get; set; }

        public MapPoint(float x, float y): this()
        {
            X = x;
            Y = y;
        }

        public float DistanceTo(MapPoint point)
        {
            return (float) Math.Sqrt(Math.Pow(point.X - X, 2) + Math.Pow(point.Y - Y, 2));
        }

        public override string ToString()
        {
            return string.Format("({0:F1}, {1:F1})", X, Y);
        }
    }
}
