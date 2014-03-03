using Mercraft.Core.Algorithms;
using UnityEngine;

namespace Mercraft.Explorer.Meshes
{
    public class PolygonMeshBuilder
    {
        public Mesh BuildMesh(Vector2[] verticies2D, float top,  float floor)
        {
            var mesh = new Mesh();
            mesh.name = "PolygonMesh";

            mesh.vertices = PolygonHelper.GetVerticies3D(verticies2D, top, floor);
            mesh.uv = PolygonHelper.GetUV(verticies2D);
            mesh.triangles = PolygonHelper.GetTriangles(verticies2D);

            mesh.RecalculateNormals();

            return mesh;
        }
    }
}
