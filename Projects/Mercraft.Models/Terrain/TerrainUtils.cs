using Mercraft.Core;
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
    }
}