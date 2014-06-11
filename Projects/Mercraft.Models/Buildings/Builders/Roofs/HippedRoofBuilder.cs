using System.Collections.Generic;
using Mercraft.Models.Buildings.Entities;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;

namespace Mercraft.Models.Buildings.Builders.Roofs
{
    public class HippedRoofBuilder : RoofBuilder
    {
        public HippedRoofBuilder(Model model, DynamicMeshGenericMultiMaterialMesh mesh)
            : base(model, mesh)
        {
        }

        public override void Build(Volume volume, Roof design)
        {
            Plan area = Model.Plan;
            int numberOfFloors = volume.NumberOfFloors;
            float baseHeight = Model.FloorHeight*numberOfFloors;
            float roofHeight = design.Height;
            int numberOfVolumePoints = volume.Points.Count;
            int subMesh = design.GetTexture();

            Vector2[] volumePoints = new Vector2[numberOfVolumePoints];
            for (int i = 0; i < numberOfVolumePoints; i++)
            {
                volumePoints[i] = area.Points[volume.Points[i]];
            }

            Vector2[][] meshData = StraightSkeleton.Calculate(volumePoints);
            Vector2[] triData = meshData[0];
            List<Vector2> interiorPoints = new List<Vector2>(meshData[1]);
            int numberOfVerts = triData.Length;
            Vector3[] verts = new Vector3[numberOfVerts];
            Vector2[] uvs = new Vector2[numberOfVerts];
            int[] tris = new int[numberOfVerts];
            for (int i = 0; i < triData.Length; i += 3)
            {
                Vector2 pa = triData[i];
                Vector2 pb = triData[i + 1];
                Vector2 pc = triData[i + 2];

                float ah = baseHeight + (interiorPoints.Contains(pa) ? roofHeight : 0);
                float bh = baseHeight + (interiorPoints.Contains(pb) ? roofHeight : 0);
                float ch = baseHeight + (interiorPoints.Contains(pc) ? roofHeight : 0);

                Vector3 v0 = new Vector3(pa.x, ah, pa.y);
                Vector3 v1 = new Vector3(pb.x, bh, pb.y);
                Vector3 v2 = new Vector3(pc.x, ch, pc.y);

                verts[i] = v0;
                verts[i + 1] = v1;
                verts[i + 2] = v2;

                Vector3 roofBaseDir = (interiorPoints.Contains(pc)) ? v1 - v0 : v2 - v1;
                Vector3 roofBaseNormal = Vector3.Cross(roofBaseDir, Vector3.up);
                Vector2[] uvsMansard = BuildingProjectUVs.Project(new Vector3[3] {v0, v1, v2}, Vector2.zero,
                    roofBaseNormal);

                uvs[i] = uvsMansard[0];
                uvs[i + 1] = uvsMansard[1];
                uvs[i + 2] = uvsMansard[2];

                tris[i] = i;
                tris[i + 1] = i + 2;
                tris[i + 2] = i + 1;
            }

            AddData(verts, uvs, tris, subMesh);
        }
    }
}