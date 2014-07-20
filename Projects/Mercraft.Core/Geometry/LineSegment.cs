using System;

namespace Mercraft.Core.Geometry
{
    public struct LineSegment
    {
        public MapPoint P1;
        public MapPoint P2;

        #region Straight line properties

        public double YIntersect
        {
            get
            {
                return P1.Y - (Gradient * P1.X);
            }
        }

        public double Gradient
        {
            get
            {
                return (P1.Y - P2.Y) / (P1.X - P2.X);
            }
        }

        #endregion

        public LineSegment(MapPoint p1, MapPoint p2)
        {
            P1 = p1;
            P2 = p2;
        }

        #region Utility methods

        /// <summary>
        /// Calculates angle between lines in degrees
        /// </summary>
        public double AngleBetween(LineSegment line)
        {
            double tanOfAngle = (line.Gradient - Gradient) / (1 + Gradient * line.Gradient);
            double angle = Math.Atan(tanOfAngle) * 180 / Math.PI;
            return angle;
        }

        #endregion
    }
}
