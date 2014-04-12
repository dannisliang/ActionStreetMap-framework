using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mercraft.Models.Buildings.Utils
{
    public class BuildingUtils
    {

        public static Vector3 ClosestPointOnLine(Vector3 a, Vector3 b, Vector3 point)
        {
            Vector3 v1 = point - a;
            Vector3 v2 = (b - a).normalized;
            float distance = Vector3.Distance(a, b);
            float t = Vector3.Dot(v2, v1);

            if (t <= 0)
                return a;
            if (t >= distance)
                return b;
            Vector3 v3 = v2 * t;
            Vector3 closestPoint = a + v3;
            return closestPoint;
        }

        public static bool FastLineIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            return (CCW(a1, b1, b2) != CCW(a2, b1, b2)) && (CCW(a1, a2, b1) != CCW(a1, a2, b2));
        }

        private static bool CCW(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return ((p2.x - p1.x) * (p3.y - p1.y) > (p2.y - p1.y) * (p3.x - p1.x));
        }
    }
}
