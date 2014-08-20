using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.World.Buildings;
using UnityEngine;

namespace Mercraft.Models.Buildings.Roofs
{
    public class FlatRoofBuilder: IRoofBuilder
    {
        public string Name { get { return "flat"; } }

        public MeshData Build(Building building, BuildingStyle style)
        {
            return new MeshData()
            {
                Vertices = GetVerticies3D(building.Footprint, building.BottomOffset + building.Height),
                Triangles = Triangulator.Triangulate(building.Footprint),
                UV = GetUV(building.Footprint),
                TextureKey = style.Roof.Texture,
                MaterialKey = style.Roof.Material
            };
        }

        private Vector3[] GetVerticies3D(MapPoint[] footprint, float top)
        {
            var length = footprint.Length;
            var vertices3D = new Vector3[length];
            for (int i = 0; i < length; i++)
            {
                vertices3D[i] = new Vector3(footprint[i].X, top, footprint[i].Y);
            }
            return vertices3D;
        }

        private Vector2[] GetUV(MapPoint[] footprint)
        {
            var uv = new Vector2[footprint.Length];

            for (int i = 0; i< uv.Length; i++)
            {
                uv[i] = new Vector2(0, 0);
            }

            return uv;
        }
    }
}
