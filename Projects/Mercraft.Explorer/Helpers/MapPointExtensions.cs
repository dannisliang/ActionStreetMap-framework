using System.Collections.Generic;
using Mercraft.Core;
using UnityEngine;

namespace Mercraft.Explorer.Helpers
{
    public static class MapPointExtensions
    {
        public static Vector3[] GetVerticies(this MapPoint[] verticies2D, float floor)
        {
            var length = verticies2D.Length;
            var verticies3D = new Vector3[length];
            for (int i = 0; i < length; i++)
            {
                verticies3D[i] = new Vector3(verticies2D[i].X, floor, verticies2D[i].Y);
            }

            return verticies3D;
        }

        public static Vector3[] GetVerticies3D(this MapPoint[] verticies2D, float top, float floor)
        {
            var length = verticies2D.Length;
            var verticies3D = new Vector3[length * 2];
            for (int i = 0; i < length; i++)
            {
                verticies3D[i] = new Vector3(verticies2D[i].X, floor, verticies2D[i].Y);
                verticies3D[i + length] = new Vector3(verticies2D[i].X, top, verticies2D[i].Y);
            }

            return verticies3D;
        }

        public static Vector2[] ToVector2(this MapPoint[] verticies2D)
        {
            var length = verticies2D.Length;
            var convertedVerticies = new Vector2[length];

            for (int i = 0; i < length; i++)
            {
                convertedVerticies[i] = new Vector2(verticies2D[i].X, verticies2D[i].Y);
            }

            return convertedVerticies;
        }
    }
}
