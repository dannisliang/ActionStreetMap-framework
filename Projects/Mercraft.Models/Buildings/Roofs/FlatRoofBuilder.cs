using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.World.Buildings;
using UnityEngine;

namespace Mercraft.Models.Buildings.Roofs
{
    /// <summary>
    ///     Builds flat roof.
    /// </summary>
    public class FlatRoofBuilder: IRoofBuilder
    {
        /// <inheritdoc />
        public string Name { get { return "flat"; } }

        /// <inheritdoc />
        public bool CanBuild(Building building)
        {
            // NOTE flat builder can be used for every type of building
            return true;
        }

        /// <inheritdoc />
        public MeshData Build(Building building, BuildingStyle style)
        {
            return new MeshData
            {
                Vertices = GetVerticies3D(building.Footprint, building.Elevation + building.MinHeight, building.Height),
                Triangles = Triangulator.Triangulate(building.Footprint),
                UV = GetUV(building.Footprint, style),
                MaterialKey = style.Roof.Path,
            };
        }

        private Vector3[] GetVerticies3D(List<MapPoint> footprint, float elevation, float height)
        {
            var length = footprint.Count;
            var vertices3D = new Vector3[length];
            
            var top = elevation + height;
            
            for (int i = 0; i < length; i++)
            {
                vertices3D[i] = new Vector3(footprint[i].X, top, footprint[i].Y);
            }
            return vertices3D;
        }

        private Vector2[] GetUV(List<MapPoint> footprint, BuildingStyle style)
        {
            var uv = new Vector2[footprint.Count];
            for (int i = 0; i < uv.Length; i++)
            {
                uv[i] = style.Roof.FrontUvMap.RightUpper;
            }

            return uv;
        }
    }
}
