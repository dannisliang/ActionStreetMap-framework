using System;
using UnityEngine;

namespace Mercraft.Models.Primitives
{
    public static class PrimitiveExtensions
    {
        public static bool SamePoint(this Vector2 point1, Vector2 point2)
        {
            double dDeffX = Math.Abs(point1.x - point2.x);
            double dDeffY = Math.Abs(point1.y - point2.y);

            return (dDeffX < ConstantValue.SmallValue) && (dDeffY < ConstantValue.SmallValue);
        }

        public static bool InLine(this Vector2 point, CLineSegment lineSegment)
        {
            bool bInline = false;

            double Ax, Ay, Bx, By, Cx, Cy;
            Bx = lineSegment.EndPoint.x;
            By = lineSegment.EndPoint.y;
            Ax = lineSegment.StartPoint.x;
            Ay = lineSegment.StartPoint.y;
            Cx = point.x;
            Cy = point.y;

            double L = lineSegment.GetLineSegmentLength();
            double s = Math.Abs(((Ay - Cy) * (Bx - Ax) - (Ax - Cx) * (By - Ay)) / (L * L));

            if (Math.Abs(s - 0) < ConstantValue.SmallValue)
            {
                if ((point.SamePoint(lineSegment.StartPoint)) || (point.SamePoint(lineSegment.EndPoint)))
                    bInline = true;
                else if ((Cx < lineSegment.GetXmax())
                    && (Cx > lineSegment.GetXmin())
                    && (Cy < lineSegment.GetYmax())
                    && (Cy > lineSegment.GetYmin()))
                    bInline = true;
            }
            return bInline;
        }

    }
}
