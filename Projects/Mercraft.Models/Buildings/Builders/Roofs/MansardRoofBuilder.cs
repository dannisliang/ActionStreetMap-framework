using System.Collections.Generic;
using Mercraft.Models.Buildings.Entities;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;
using Texture = Mercraft.Models.Buildings.Entities.Texture;

namespace Mercraft.Models.Buildings.Builders.Roofs
{
    public class MansardRoofBuilder : RoofBuilder
    {
        public MansardRoofBuilder(Model model, DynamicMultiMaterialMesh mesh)
            : base(model, mesh)
        {
        }

        public override void Build(Volume volume, Roof design)
        {
            Plan area = Model.Plan;
            int numberOfVolumePoints = volume.Points.Count;
            int numberOfFloors = volume.NumberOfFloors;
            float floorHeight = Model.FloorHeight;
            Vector3 volumeFloorHeight = Vector3.up*(numberOfFloors*floorHeight);

            //add top base of the flat roof
            Vector3[] topVerts = new Vector3[numberOfVolumePoints];
            Vector2[] topUVs = new Vector2[numberOfVolumePoints];
            int topTextureID = design.GetTexture();
            Texture texture = Textures[topTextureID];

            for (int l = 0; l < numberOfVolumePoints; l++)
            {
                int indexA, indexB, indexA0, indexB0;
                Vector3 p0, p1, p00, p10;
                indexA = l;
                indexB = (l < numberOfVolumePoints - 1) ? l + 1 : 0;
                indexA0 = (l > 0) ? l - 1 : numberOfVolumePoints - 1;
                indexB0 = (l < numberOfVolumePoints - 2) ? l + 2 : l + 2 - numberOfVolumePoints;

                p0 = area.Points[volume.Points[indexA]].Vector3();
                p1 = area.Points[volume.Points[indexB]].Vector3();
                p00 = area.Points[volume.Points[indexA0]].Vector3();
                p10 = area.Points[volume.Points[indexB0]].Vector3();

                float facadeWidth = Vector3.Distance(p0, p1);
                Vector3 facadeDirection = (p1 - p0).normalized;
                Vector3 facadeDirectionLeft = (p0 - p00).normalized;
                Vector3 facadeDirectionRight = (p10 - p1).normalized;
                Vector3 facadeNormal = Vector3.Cross(facadeDirection, Vector3.up);
                Vector3 facadeNormalLeft = Vector3.Cross(facadeDirectionLeft, Vector3.up);
                Vector3 facadeNormalRight = Vector3.Cross(facadeDirectionRight, Vector3.up);

                float roofHeight = design.Height;
                float baseDepth = design.FloorDepth;
                float cornerLeftRad = Vector3.Angle(facadeDirection, -facadeDirectionLeft)*Mathf.Deg2Rad/2;
                float cornerRightRad = Vector3.Angle(-facadeDirection, facadeDirectionRight)*Mathf.Deg2Rad/2;
                float cornerDepthLeft = baseDepth/Mathf.Sin(cornerLeftRad);
                float cornerDepthRight = baseDepth/Mathf.Sin(cornerRightRad);
                float topDepth = design.Depth;
                float cornerTopDepthLeft = topDepth/Mathf.Sin(cornerLeftRad);
                float cornerTopDepthRight = topDepth/Mathf.Sin(cornerRightRad);

                Vector3 pr = facadeDirection*facadeWidth;

                Vector3 leftDir = (facadeNormal + facadeNormalLeft).normalized;
                Vector3 rightDir = (facadeNormal + facadeNormalRight).normalized;

                p0 += volumeFloorHeight;


                Vector3 w0, w1, w2, w3, w4, w5;
                w0 = p0;
                w1 = p0 + pr;
                w2 = w0 + leftDir*cornerDepthLeft;
                w3 = w1 + rightDir*cornerDepthRight;
                w4 = w2 + leftDir*cornerTopDepthLeft + Vector3.up*roofHeight;
                w5 = w3 + rightDir*cornerTopDepthRight + Vector3.up*roofHeight;

                Vector3[] verts = new Vector3[6] {w0, w1, w2, w3, w4, w5};
                List<Vector2> uvs = new List<Vector2>();

                Vector2[] uvsFloor = BuildingProjectUVs.Project(new Vector3[4] {w0, w1, w2, w3}, Vector2.zero,
                    facadeNormal);
                Vector2[] uvsMansard = BuildingProjectUVs.Project(new Vector3[3] {w2, w4, w5}, uvsFloor[2], facadeNormal);
                uvs.AddRange(uvsFloor);
                uvs.Add(uvsMansard[1]);
                uvs.Add(uvsMansard[2]);

                Vector3[] vertsA = new Vector3[4] {verts[0], verts[1], verts[2], verts[3]};
                Vector2[] uvsA = new Vector2[4] {uvsFloor[0], uvsFloor[1], uvsFloor[2], uvsFloor[3]};
                int[] trisA = new int[6] {1, 0, 2, 1, 2, 3};
                int subMeshA = design.GetTexture();
                Mesh.AddData(vertsA, uvsA, trisA, subMeshA);

                Vector3[] vertsB = new Vector3[4] {verts[2], verts[3], verts[4], verts[5]};
                Vector2[] uvsB = new Vector2[4] {uvsFloor[2], uvsFloor[3], uvsMansard[1], uvsMansard[2]};
                int[] trisB = new int[6] {0, 2, 1, 1, 2, 3};
                int subMeshB = design.GetTexture();
                Mesh.AddData(vertsB, uvsB, trisB, subMeshB);

                //modify point for the top geometry
                Vector2 point = area.Points[volume.Points[l]];
                topVerts[l] = point.Vector3() + volumeFloorHeight + Vector3.up*roofHeight +
                              leftDir*(cornerDepthLeft + cornerTopDepthLeft);
                topUVs[l] = new Vector2(topVerts[l].x/texture.TextureUnitSize.x, topVerts[l].z/texture.TextureUnitSize.y);
            }

            Vector2[] topVertV2z = new Vector2[topVerts.Length];
            for (int i = 0; i < topVerts.Length; i++)
                topVertV2z[i] = new Vector2(topVerts[i].x, topVerts[i].z);
            int[] topTris = EarClipper.Triangulate(topVertV2z);
            AddData(topVerts, topUVs, topTris, topTextureID); //top

            if (design.HasDormers)
                Dormers(volume, design);
        }
    }
}