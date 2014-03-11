using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Infrastructure.Config;
using UnityEngine;

namespace Mercraft.Explorer.Meshes
{
    public class PolygonMeshBuilder: IMeshBuilder, IConfigurable
    {
        private const string MeshBuilderNameKey = "@name";

        public string Name { get; private set; }

        public Mesh Build(Vector2[] verticies, Model model, Rule rule)
        {
            var mesh = new Mesh();
            mesh.name = "PolygonMesh";

            // TODO implement floor calc algorithm
            var floor = 1;
            var height = rule.Evaluate<int>(model, "height");
            var top = floor + height;

            mesh.vertices = PolygonHelper.GetVerticies3D(verticies, top, floor);
            mesh.uv = PolygonHelper.GetUV(verticies);
            mesh.triangles = PolygonHelper.GetTriangles(verticies);

            mesh.RecalculateNormals();

            return mesh;
        }

        public void Configure(IConfigSection configSection)
        {
            Name = configSection.GetString(MeshBuilderNameKey);
        }
    }
}
