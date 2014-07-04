using Mercraft.Core;
using Mercraft.Models.Terrain.Roads;
using UnityEngine;

namespace Mercraft.Models.Terrain
{
    public class TerrainUtils
    {
        public static Vector2 ConvertWorldToTerrain(MapPoint worldPos, Vector2 terrainPosition, float widthRatio, float heightRatio)
        {
            return ConvertWorldToTerrain(worldPos.X, worldPos.Y, terrainPosition, widthRatio, heightRatio);
        }

        public static Vector2 ConvertWorldToTerrain(Vector2 worldPos, Vector2 terrainPosition, float widthRatio, float heightRatio)
        {
            return ConvertWorldToTerrain(worldPos.x, worldPos.y, terrainPosition, widthRatio, heightRatio);
        }

        private static Vector2 ConvertWorldToTerrain(float x, float y, Vector2 terrainPosition, float widthRatio, float heightRatio)
        {
            return new Vector2
            {
                // NOTE Coords are inverted here!
                y = (x - terrainPosition.x) * widthRatio,
                x = (y - terrainPosition.y) * heightRatio
            };
        }

        public static bool IsPointInPolygon(Vector2[] polygon, Vector2 point)
        {
            bool isInside = false;
            int j = polygon.Length - 1;
            for (int i = 0; i < polygon.Length; i++)
            {
                if ((polygon[i].y < point.y && polygon[j].y >= point.y || polygon[j].y < point.y && polygon[i].y >= point.y)
                && (polygon[i].x + (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) * (polygon[j].x - polygon[i].x) < point.x))
                {
                    isInside = !isInside;
                }
                j = i;
            }
            return isInside;
        }

        public static Vector2 IntersectionPoint(Segment first, Segment second)
        {
            float A1 = first.End.y - first.Start.y;
            float B1 = first.Start.x - first.End.x;
            float C1 = A1 * first.Start.x + B1 * first.Start.y;

            // Get A,B,C of second line - points : ps2 to pe2
            float A2 = second.End.y - second.Start.y;
            float B2 = second.Start.x - second.End.x;
            float C2 = A2 * second.Start.x + B2 * second.Start.y;

            // Get delta and check if the lines are parallel
            float delta = A1 * B2 - A2 * B1;
            if (delta == 0)
                throw new System.Exception("Lines are parallel");

            // now return the Vector2 intersection point
            return new Vector2(
                (B2 * C1 - B1 * C2) / delta,
                (A1 * C2 - A2 * C1) / delta
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

            if (alphaDenominator == 0 || betaDenominator == 0)
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