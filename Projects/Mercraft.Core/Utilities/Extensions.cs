using UnityEngine;

namespace Mercraft.Core.Utilities
{
    public static class Extensions
    {
        public static bool AreSame(this Vector2 point1, Vector2 point2)
        {
            return MathUtility.AreEqual(point1.x, point2.x) && MathUtility.AreEqual(point1.y, point2.y);
        }
    }
}
