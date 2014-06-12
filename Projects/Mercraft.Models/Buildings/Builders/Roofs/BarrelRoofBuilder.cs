using Mercraft.Models.Buildings.Entities;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;

namespace Mercraft.Models.Buildings.Builders.Roofs
{
    public class BarrelRoofBuilder: RoofBuilder
    {
        public BarrelRoofBuilder(Model model, DynamicMultiMaterialMesh mesh): base(model, mesh)
        {
        }

        public override void Build(Volume volume, Roof design)
        {
            Plan area = Model.Plan;
            int numberOfFloors = volume.NumberOfFloors;
            float floorHeight = Model.FloorHeight;
            Vector3 volumeFloorHeight = Vector3.up * (numberOfFloors * floorHeight);

            Vector3[] points = new Vector3[4];
            
            if (design.Direction == 0)
            {
                points[0] = area.Points[volume.Points[0]].Vector3() + volumeFloorHeight;
                points[1] = area.Points[volume.Points[3]].Vector3() + volumeFloorHeight;
                points[2] = area.Points[volume.Points[1]].Vector3() + volumeFloorHeight;
                points[3] = area.Points[volume.Points[2]].Vector3() + volumeFloorHeight;
            }
            else
            {
                points[0] = area.Points[volume.Points[0]].Vector3() + volumeFloorHeight;
                points[1] = area.Points[volume.Points[1]].Vector3() + volumeFloorHeight;
                points[2] = area.Points[volume.Points[2]].Vector3() + volumeFloorHeight;
                points[3] = area.Points[volume.Points[3]].Vector3() + volumeFloorHeight;
            }

            int barrelSegments = design.BarrelSegments + 1;
            Vector3[] bPoints = new Vector3[barrelSegments * 2];
            for (int i = 0; i < barrelSegments; i++)
            {
                float lerp = i / (float)(barrelSegments - 1);
                Vector3 height = Mathf.Sin(lerp * Mathf.PI) * design.Height * Vector3.up;
                float cosLerp = 1 - (Mathf.Cos((lerp) * Mathf.PI) + 1) / 2;
                bPoints[i] = Vector3.Lerp(points[0], points[1], cosLerp) + height;
                bPoints[i + barrelSegments] = Vector3.Lerp(points[2], points[3], cosLerp) + height;
            }

            int topIterations = barrelSegments - 1;
            int subMesh = design.GetTexture();
            bool flipped = design.IsFlipped();
            for (int t = 0; t < topIterations; t++)
                AddPlane(bPoints[t + 1], bPoints[t], bPoints[t + barrelSegments + 1], bPoints[t + barrelSegments], subMesh, flipped);//top

            Vector3 centerA = Vector3.Lerp(points[0], points[1], 0.5f);
            Vector3 centerB = Vector3.Lerp(points[2], points[3], 0.5f);
            for (int e = 0; e < topIterations; e++)
            {
                float lerpA = (e / (float)(topIterations)) * Mathf.PI;
                float lerpB = ((e + 1) / (float)(topIterations)) * Mathf.PI;
                Vector2[] uvs = new Vector2[3]{
                    new Vector2(0.5f,0),
                    new Vector2(1-(Mathf.Cos(lerpA)+1)/2,Mathf.Sin(lerpA)),
                    new Vector2(1-(Mathf.Cos(lerpB)+1)/2,Mathf.Sin(lerpB))
                };

                Vector3[] verts = new Vector3[3] { centerA, bPoints[e], bPoints[e + 1] };
                int[] tri = new int[3] { 0, 2, 1 };
                AddData(verts, uvs, tri, design.GetTexture());

                verts = new Vector3[3] { centerB, bPoints[e + barrelSegments], bPoints[e + 1 + barrelSegments] };
                tri = new int[3] { 0, 1, 2 };
                AddData(verts, uvs, tri, design.GetTexture());
            }
        }
    }
}