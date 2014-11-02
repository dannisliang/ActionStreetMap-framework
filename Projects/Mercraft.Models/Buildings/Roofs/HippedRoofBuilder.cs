using System.Linq;
using Mercraft.Core.Scene.World.Buildings;
using Mercraft.Models.Geometry.Polygons;
using UnityEngine;

namespace Mercraft.Models.Buildings.Roofs
{
    /// <summary>
    ///     Builds hipped roof.
    /// </summary>
    public class HippedRoofBuilder: IRoofBuilder
    {
        /// <inheritdoc />
        public virtual string Name { get { return "hipped"; } }

        /// <inheritdoc />
        public virtual bool CanBuild(Building building)
        {
            return true;
        }

        /// <inheritdoc />
        public virtual MeshData Build(Building building, BuildingStyle style)
        {
            var height = building.Footprint[0].Elevation + building.Height;
            var roofHeight = building.RoofHeight + height;

            var skeleton = StraightSkeleton.Calculate(building.Footprint);
            
            var skeletVertices = skeleton.Item1;
            skeletVertices.Reverse();

            var vertices = new Vector3[skeletVertices.Count];
            var triangles = new int[skeletVertices.Count];
            var uv = new Vector2[skeletVertices.Count];

            for (int i = 0; i < skeletVertices.Count; i++)
            {
                var vertex = skeletVertices[i];
                var y = skeleton.Item2.Any(t => vertex == t) ? roofHeight : height;
                vertices[i].Set(vertex.x, y, vertex.y);
                triangles[i] = i;
                uv[i] = style.Roof.FrontUvMap.RightUpper;
            }
           
            return new MeshData()
            {
                Vertices = vertices,
                Triangles = triangles,
                UV = uv,
                MaterialKey = style.Roof.Path,
            };
        }
    }
}
