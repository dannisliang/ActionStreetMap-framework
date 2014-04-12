using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercraft.Models.Buildings.Entities;
using Mercraft.Models.Buildings.Utils;

namespace Mercraft.Models.Buildings.Builders
{
    using UnityEngine;
    using System.Collections.Generic;

    public class BuilderBuildingBox
    {
        private static Data data;
        private static DynamicMeshGenericMultiMaterialMesh mesh;

        public static void Build(DynamicMeshGenericMultiMaterialMesh _mesh, Data _data)
        {
            data = _data;
            mesh = _mesh;
            Plan plan = data.Plan;

            int numberOfVolumes = data.Plan.numberOfVolumes;
            for (int s = 0; s < numberOfVolumes; s++)
            {
                Volume volume = plan.volumes[s];
                int numberOfVolumePoints = volume.points.Count;
                Vector3[] newEndVerts = new Vector3[numberOfVolumePoints];
                Vector2[] newEndUVs = new Vector2[numberOfVolumePoints];
                Vector3 volumeHeight = Vector3.up * (volume.numberOfFloors * data.FloorHeight);
                for (int i = 0; i < numberOfVolumePoints; i++)
                {
                    newEndVerts[i] = plan.points[volume.points[i]].Vector3() + volumeHeight;
                    newEndUVs[i] = Vector2.zero;
                }

                List<int> tris = new List<int>(data.Plan.GetTrianglesBySectorBase(s));
                mesh.AddData(newEndVerts, newEndUVs, tris.ToArray(), 0);
            }
            //Build ROOF

            //Build facades
            for (int v = 0; v < numberOfVolumes; v++)
            {
                Volume volume = plan.volumes[v];
                int numberOfVolumePoints = volume.points.Count;

                for (int f = 0; f < numberOfVolumePoints; f++)
                {
                    if (!volume.renderFacade[f])
                        continue;

                    int indexA = f;
                    int indexB = (f < numberOfVolumePoints - 1) ? f + 1 : 0;
                    Vector3 p0 = plan.points[volume.points[indexA]].Vector3();
                    Vector3 p1 = plan.points[volume.points[indexB]].Vector3();

                    int floorBase = plan.GetFacadeFloorHeight(v, volume.points[indexA], volume.points[indexB]);
                    int numberOfFloors = volume.numberOfFloors - floorBase;
                    if (numberOfFloors < 1)
                    {
                        //no facade - adjacent facade is taller and covers this one
                        continue;
                    }
                    float floorHeight = data.FloorHeight;

                    Vector3 floorHeightStart = Vector3.up * (floorBase * floorHeight);
                    Vector3 wallHeight = Vector3.up * (volume.numberOfFloors * floorHeight) - floorHeightStart;
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

            data = null;
            mesh = null;
        }
    }
}
