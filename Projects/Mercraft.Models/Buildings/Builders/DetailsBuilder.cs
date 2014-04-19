using System.Collections.Generic;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;

namespace Mercraft.Models.Buildings.Entities
{
    public class DetailsBuilder
    {
        private static Material detailMat;
        private static Texture2D detailtexture;

        /// <summary>
        /// Generate an array of gameobjects that contain all the generated detail meshes
        /// </summary>
        public static GameObject[] Render(DynamicMeshGenericMultiMaterialMesh mesh, Data data)
        {
            List<GameObject> detailGameobjects = new List<GameObject>();
            int numberOfDetails = data.Details.Count;

            if (numberOfDetails == 0)
                return detailGameobjects.ToArray();

            var detailMeshes = Build(mesh, data);

            int numberOfMeshes = detailMeshes.Count;
            if (numberOfMeshes == 0)
                return detailGameobjects.ToArray();

            if (detailMat == null)
                detailMat = new Material(Shader.Find("Diffuse"));

            detailMat.mainTexture = detailtexture;
            for (int i = 0; i < numberOfMeshes; i++)
            {
                GameObject details = new GameObject("details " + i);
                details.AddComponent<MeshFilter>().mesh = detailMeshes[i];
                details.AddComponent<MeshRenderer>().sharedMaterial = detailMat;
                detailGameobjects.Add(details);
            }
            return detailGameobjects.ToArray();
        }

        /// <summary>
        /// Generate the detail meshes and return the export object
        /// </summary>
        public static List<Mesh> Build(DynamicMeshGenericMultiMaterialMesh mesh, Data data)
        {
            var detailTextures = new List<Texture2D>();
            var detailSubmeshesWithTextures = new List<int>();
            int numberOfDetails = data.Details.Count;
            mesh.Clear();
            mesh.SubMeshCount = numberOfDetails;

            for (int d = 0; d < numberOfDetails; d++)
            {
                Detail detail = data.Details[d];
                if (detail.Mesh == null)
                    continue;
                int faceIndex = detail.Face;
                Vector3 position = Vector3.zero;
                Plan plan = data.Plan;
                int numberOfVolumes = plan.Volumes.Count;
                Vector2 faceUv = detail.FaceUv;
                Quaternion faceAngle = Quaternion.identity;
                if (detail.Place == Detail.DetailPlace.Facade)
                {
                    //find facade
                    int facadeCount = 0;
                    bool facadeFound = false;
                    for (int s = 0; s < numberOfVolumes; s++)
                    {
                        Volume volume = plan.Volumes[s];
                        int numberOfVolumePoints = volume.Points.Count;
                        for (int p = 0; p < numberOfVolumePoints; p++)
                        {
                            if (facadeCount == faceIndex)
                            {
                                int indexA = p;
                                int indexB = (p + 1) % numberOfVolumePoints;
                                Vector3 p0 = plan.Points[volume.Points[indexA]].Vector3();
                                Vector3 p1 = plan.Points[volume.Points[indexB]].Vector3();
                                Vector3 basePosition = Vector3.Lerp(p0, p1, faceUv.x);
                                Vector3 detailHeight = Vector3.up * (volume.NumberOfFloors * data.FloorHeight * faceUv.y);
                                Vector3 facadeCross = Vector3.Cross(Vector3.up, p1 - p0).normalized;
                                Vector3 detailDepth = facadeCross * detail.FaceHeight;
                                faceAngle = Quaternion.LookRotation(facadeCross);
                                position = basePosition + detailHeight + detailDepth;
                                facadeFound = true;
                                break;
                            }
                            facadeCount++;
                        }
                        if (facadeFound)
                            break;
                    }
                }
                else
                {
                    Volume volume = plan.Volumes[Mathf.Clamp(0, numberOfVolumes - 1, faceIndex)];
                    int numberOfVolumePoints = volume.Points.Count;
                    Vector3 minimumRoofPoint = plan.Points[volume.Points[0]].Vector3();
                    Vector3 maximumRoofPoint = minimumRoofPoint;
                    for (int p = 1; p < numberOfVolumePoints; p++)
                    {
                        Vector3 p0 = plan.Points[volume.Points[p]].Vector3();
                        if (p0.x < minimumRoofPoint.x) minimumRoofPoint.x = p0.x;
                        if (p0.z < minimumRoofPoint.y) minimumRoofPoint.y = p0.z;
                        if (p0.x > maximumRoofPoint.x) maximumRoofPoint.x = p0.x;
                        if (p0.z > maximumRoofPoint.y) maximumRoofPoint.y = p0.z;
                    }
                    position.x = Mathf.Lerp(minimumRoofPoint.x, maximumRoofPoint.x, faceUv.x);
                    position.z = Mathf.Lerp(minimumRoofPoint.y, maximumRoofPoint.y, faceUv.y);
                    position.y = volume.NumberOfFloors * data.FloorHeight + detail.FaceHeight;
                }

                Quaternion userRotation = Quaternion.Euler(detail.UserRotation);
                int vertexCount = detail.Mesh.vertexCount;
                var verts = new Vector3[vertexCount];
                Quaternion rotate = faceAngle * userRotation;
                for (int i = 0; i < vertexCount; i++)
                {
                    Vector3 sourceVertex = Vector3.Scale(detail.Mesh.vertices[i], detail.Scale);
                    Vector3 outputVertex = (rotate) * sourceVertex + position;
                    verts[i] = outputVertex;
                }
                mesh.AddData(verts, detail.Mesh.uv, detail.Mesh.triangles, d);
                detail.WorldPosition = position;
                detail.WorldRotation = rotate;
            }

            if (detailtexture != null)
                Object.DestroyImmediate(detailtexture);

            var outputMeshes = new List<Mesh>();
            if (detailSubmeshesWithTextures.Count > 0)
            {
                Rect[] textureRects = BuildingTexturePacker.Pack(out detailtexture, detailTextures.ToArray(), 512);
                if (detailSubmeshesWithTextures.Count > 0) mesh.Atlas(detailSubmeshesWithTextures.ToArray(), textureRects);
                mesh.CollapseSubmeshes();
                mesh.Build();
                int numberOfMeshes = mesh.MeshCount;
                for (int i = 0; i < numberOfMeshes; i++)
                    outputMeshes.Add(mesh[i].Mesh);
            }

            return outputMeshes;
        }
    }
}
