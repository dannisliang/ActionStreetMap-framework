using UnityEngine;

namespace Mercraft.Models.Buildings.Utils
{
    public static class VectorExtensions
    {
        public static Vector3 Vector3(this Vector2 vector2)
        {
            return new Vector3(vector2.x, 0, vector2.y);
        }
    }
}