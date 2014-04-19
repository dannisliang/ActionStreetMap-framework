using Mercraft.Models.Buildings.Entities;
using Mercraft.Models.Buildings.Utils;

namespace Mercraft.Models.Buildings.Builders
{
    using UnityEngine;
    using System.Collections.Generic;

    public class BuildingBoxBuilder
    {
        public static void Build(DynamicMeshGenericMultiMaterialMesh mesh, Data data)
        {
            Plan plan = data.Plan;
            int numberOfVolumes = data.Plan.Volumes.Count;
            for (int s = 0; s < numberOfVolumes; s++)
            {
                Volume volume = plan.Volumes[s];
                int numberOfVolumePoints = volume.Points.Count;
                var newEndVerts = new Vector3[numberOfVolumePoints];
                var newEndUVs = new Vector2[numberOfVolumePoints];
                var volumeHeight = Vector3.up * (volume.NumberOfFloors * data.FloorHeight);
                for (int i = 0; i < numberOfVolumePoints; i++)
                {
                    newEndVerts[i] = plan.Points[volume.Points[i]].Vector3() + volumeHeight;
                    newEndUVs[i] = Vector2.zero;
                }

                List<int> tris = new List<int>(data.Plan.GetTrianglesBySectorBase(s));
                mesh.AddData(newEndVerts, newEndUVs, tris.ToArray(), 0);
            }

            //Build facades
            for (int v = 0; v < numberOfVolumes; v++)
            {
                Volume volume = plan.Volumes[v];
                int numberOfVolumePoints = volume.Points.Count;

                for (int f = 0; f < numberOfVolumePoints; f++)
                {
                    if (!volume.RenderFacade[f])
                        continue;

                    int indexA = f;
                    int indexB = (f < numberOfVolumePoints - 1) ? f + 1 : 0;
                    Vector3 p0 = plan.Points[volume.Points[indexA]].Vector3();
                    Vector3 p1 = plan.Points[volume.Points[indexB]].Vector3();

                    int floorBase = plan.GetFacadeFloorHeight(v, volume.Points[indexA], volume.Points[indexB]);
                    int numberOfFloors = volume.NumberOfFloors - floorBase;
                    if (numberOfFloors < 1)
                    {
                        //no facade - adjacent facade is taller and covers this one
                        continue;
                    }
                    float floorHeight = data.FloorHeight;

                    Vector3 floorHeightStart = Vector3.up * (floorBase * floorHeight);
                    Vector3 wallHeight = Vector3.up * (volume.NumberOfFloors * floorHeight) - floorHeightStart;
                    float facadeWidth = Vector3.Distance(p0, p1);


                    p0 += floorHeightStart;
                    p1 += floorHeightStart;

                    Vector3 w0 = p0;
                    Vector3 w1 = p1;
                    Vector3 w2 = w0 + wallHeight;
                    Vector3 w3 = w1 + wallHeight;

                    Vector2 uvMin = new Vector2(0, 0);
                    Vector2 uvMax = new Vector2(facadeWidth, floorHeight);

                    mesh.AddPlane(w0, w1, w2, w3, uvMin, uvMax, 0);
                }
            }
        }
    }
}
