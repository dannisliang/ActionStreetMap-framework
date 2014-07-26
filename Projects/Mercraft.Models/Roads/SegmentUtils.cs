using System;
using UnityEngine;

namespace Mercraft.Models.Roads
{
    public class SegmentUtils
    {
        public static Vector2 IntersectionPoint(Segment first, Segment second)
        {
            float a1 = first.End.y - first.Start.y;
            float b1 = first.Start.x - first.End.x;
            float c1 = a1 * first.Start.x + b1 * first.Start.y;

            // Get A,B,C of second line - points : ps2 to pe2
            float a2 = second.End.y - second.Start.y;
            float b2 = second.Start.x - second.End.x;
            float c2 = a2 * second.Start.x + b2 * second.Start.y;

            // Get delta and check if the lines are parallel
            float delta = a1 * b2 - a2 * b1;
            if (Math.Abs(delta) < float.MinValue)
                throw new System.Exception("Segments are parallel");

            // now return the Vector2 intersection point
            return new Vector2(
                (b2 * c1 - b1 * c2) / delta,
                (a1 * c2 - a2 * c1) / delta
            );
        }

        public static bool Intersect(Segment first, Segment second)
        {
            Vector2 a = first.End - first.Start;
            Vector2 b = second.Start - second.End;
            Vector2 c = first.Start - second.Start;

            float alphaNumerator = b.y * c.x - b.x * c.y;
            float alphaDenominator = a.y * b.x - a.x * b.y;
            float betaNumerator = a.x * c.y - a.y * c.x;
            float betaDenominator = alphaDenominator;

            bool doIntersect = true;

            if (Math.Abs(alphaDenominator) < float.MinValue || Math.Abs(betaDenominator) < float.MinValue)
            {
                doIntersect = false;
            }
            else
            {

                if (alphaDenominator > 0)
                {
                    if (alphaNumerator < 0 || alphaNumerator > alphaDenominator)
                    {
                        doIntersect = false;
                    }
                }
                else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator)
                {
                    doIntersect = false;
                }

                if (doIntersect && betaDenominator > 0)
                {
                    if (betaNumerator < 0 || betaNumerator > betaDenominator)
                    {
                        doIntersect = false;
                    }
                }
                else if (betaNumerator > 0 || betaNumerator < betaDenominator)
                {
                    doIntersect = false;
                }
            }

            return doIntersect;
        }
    }
}
