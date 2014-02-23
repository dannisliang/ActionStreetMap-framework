using System;
using System.Collections.Generic;
using Mercraft.Models.Primitives;
using UnityEngine;

namespace Mercraft.Models.Algorithms
{
    public class PolygonTriangulation
    {
        public static int[] GetTriangles(Vector2[] verticies)
        {
            var ps = new PolygonShape(verticies);
            ps.CutEar();

            /*Func<Vector2, int> getIndex = v =>
            {
                for (int i = 0; i < verticies.Length; i++)
                {
                    if (v == verticies[i])
                        return i;
                }
                throw new InvalidOperationException("Vertice not found!");
            };*/

            var triangleIndicies = new List<int>();
            for (int i = 0; i < ps.NumberOfPolygons; i++)
            {
                // NOTE: should be always 3!!!
                int nPoints = ps.Polygons(i).Length;
         
                for (int j = 0; j < nPoints; j++)
                {
                    triangleIndicies.Add(
                        Array.FindIndex(verticies, v => v == ps.Polygons(i)[j]));
                }
            }

            return triangleIndicies.ToArray();
        }
    }
}
