using UnityEngine;

namespace Mercraft.Models.Utils
{
    public class PolygonUtils
    {
        /// <summary>
        ///     Determines whether point is inside polygon
        ///     Even odd algorythm
        ///     http://en.wikipedia.org/wiki/Even-odd_rule
        /// </summary>
        public static bool IsPointInPolygon(Vector2[] polygon, Vector2 point)
        {
            bool isInside = false;
            int j = polygon.Length - 1;
            for (int i = 0; i < polygon.Length; i++)
            {
                if ((polygon[i].y < point.y && polygon[j].y >= point.y || polygon[j].y < point.y && polygon[i].y >= point.y)
                    && (polygon[i].x + (point.y - polygon[i].y)/(polygon[j].y - polygon[i].y)*(polygon[j].x - polygon[i].x) < point.x))
                {
                    isInside = !isInside;
                }
                j = i;
            }
            return isInside;
        }
    }
}