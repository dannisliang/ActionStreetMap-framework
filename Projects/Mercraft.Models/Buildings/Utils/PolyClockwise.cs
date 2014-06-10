using UnityEngine;

namespace Mercraft.Models.Buildings.Utils
{
    /// <summary>
    ///     Checks if the provided points follow a clockwise winding
    ///     used to ensure that generated faces render correctly
    /// </summary>
    public class PolyClockwise
    {
        public static bool Check(Vector2[] points)
        {
            int numberOfPoints = points.Length;
            int i, j, k;
            int count = 0;
            float z;

            if (numberOfPoints < 3)
                return (false);

            for (i = 0; i < numberOfPoints; i++)
            {
                j = (i + 1)%numberOfPoints;
                k = (i + 2)%numberOfPoints;

                Vector2 pointA = points[i];
                Vector2 pointB = points[j];
                Vector2 pointC = points[k];

                z = (pointB.x - pointA.x)*(pointC.y - pointA.y);
                z -= (pointB.y - pointA.y)*(pointC.x - pointA.x);

                if (z < 0)
                    count--;
                else if (z > 0)
                    count++;
            }

            if (count > 0)
                return (true);
            if (count < 0)
                return (false);
            return (false);
        }
    }
}