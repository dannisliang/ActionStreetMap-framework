using System.Collections.Generic;
using Mercraft.Models.Buildings.Entities;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;
using Texture = Mercraft.Models.Buildings.Entities.Texture;

namespace Mercraft.Models.Buildings.Builders.Roofs
{
    public class FlatRoofBuilder : RoofBuilder
    {
        public FlatRoofBuilder(Model model, DynamicMultiMaterialMesh mesh)
            : base(model, mesh)
        {
        }

        public override void Build(Volume volume, Roof design)
        {
            Plan area = Model.Plan;
            int volumeIndex = area.Volumes.IndexOf(volume);
            int numberOfVolumePoints = volume.Points.Count;
            int numberOfFloors = volume.NumberOfFloors;
            float floorHeight = Model.FloorHeight;
            Vector3 volumeFloorHeight = Vector3.up*(numberOfFloors*floorHeight);

            //add top base of the flat roof
            Vector3[] newEndVerts = new Vector3[numberOfVolumePoints];
            Vector2[] newEndUVs = new Vector2[numberOfVolumePoints];
            int[] tris = new List<int>(area.GetTrianglesBySectorBase(volumeIndex)).ToArray();
            int roofTextureID = design.GetTexture();
            Texture texture = Model.Textures[roofTextureID];
            for (int i = 0; i < numberOfVolumePoints; i++)
            {
                Vector2 point = area.Points[volume.Points[i]];
                newEndVerts[i] = point.Vector3() + volumeFloorHeight;
                newEndUVs[i] = new Vector2(point.x/texture.TextureUnitSize.x, point.y/texture.TextureUnitSize.y);
            }

            AddData(newEndVerts, newEndUVs, tris, design.GetTexture());
        }
    }
}