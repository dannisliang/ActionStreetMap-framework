using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.World.Buildings;
using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Models.Buildings.Roofs
{
    public class FlatRoofBuilder: IRoofBuilder
    {
        public string Name { get { return "flat"; } }

        public bool CanBuild(Building building)
        {
            // NOTE flat builder can be used for every type of building
            return true;
        }

        public MeshData Build(Building building, BuildingStyle style)
        {
            return new MeshData()
            {
                Vertices = GetVerticies3D(building.Footprint, building.Elevation + building.MinHeight, building.Height),
                Triangles = Triangulator.Triangulate(building.Footprint),
                UV = GetUV(building.Footprint, style),
                TextureKey = style.Roof.Textures[RandomHelper.GetIndex(building.Id, style.Roof.Textures.Length)],
                MaterialKey = style.Roof.Materials[RandomHelper.GetIndex(building.Id, style.Roof.Materials.Length)]
            };
        }

        private Vector3[] GetVerticies3D(MapPoint[] footprint, float elevation, float height)
        {
            var length = footprint.Length;
            var vertices3D = new Vector3[length];
            
            var top = elevation + height;
            
            for (int i = 0; i < length; i++)
            {
                vertices3D[i] = new Vector3(footprint[i].X, top, footprint[i].Y);
            }
            return vertices3D;
        }

        private Vector2[] GetUV(MapPoint[] footprint, BuildingStyle style)
        {
            // TODO find better way to define uv mapping
            // TODO define constant in different place
            var uv = new Vector2[footprint.Length];
            for (int i = 0; i < uv.Length; i++)
            {
                uv[i] = new Vector2(footprint[i].X / style.Roof.UnitSize, footprint[i].Y / style.Roof.UnitSize);
            }

            return uv;
        }
    }
}
