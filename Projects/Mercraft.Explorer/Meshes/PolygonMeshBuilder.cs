using System.Collections.Generic;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using UnityEngine;

namespace Mercraft.Explorer.Meshes
{
    public class PolygonMeshBuilder: IMeshBuilder
    {
        public Mesh Build(Vector2[] verticies, Model model, Rule rule)
        {
            var mesh = new Mesh();
            mesh.name = "PolygonMesh";

            var top = rule.Evaluate<float>(model, "height");

            // TODO calc floor position
            var floor = 0;

            mesh.vertices = PolygonHelper.GetVerticies3D(verticies, top, floor);
            mesh.uv = PolygonHelper.GetUV(verticies);
            mesh.triangles = PolygonHelper.GetTriangles(verticies);

            mesh.RecalculateNormals();

            return mesh;
        }
    }
}
