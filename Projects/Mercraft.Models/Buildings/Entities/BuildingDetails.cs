using System.Collections.Generic;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;

namespace Mercraft.Models.Buildings.Entities
{
    public class BuildingDetailExportObject
    {
        public Mesh[] detailMeshes = new Mesh[0];
        public Texture2D texture = null;
    }

    public class BuildingDetails
    {
        //    private static float timestart;
        private static Material detailMat;
        private static Texture2D detailtexture;

        /// <summary>
        /// generate an array of gameobjects that contain all the generated detail meshes - ready to display in a scene
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static GameObject[] Render(DynamicMeshGenericMultiMaterialMesh mesh, Data data)
        {
            List<GameObject> detailGameobjects = new List<GameObject>();
            int numberOfDetails = data.Details.Count;

            if (numberOfDetails == 0)
                return detailGameobjects.ToArray();

            BuildingDetailExportObject exportObject = Build(mesh, data);


            int numberOfMeshes = exportObject.detailMeshes.Length;
            if (numberOfMeshes == 0)
                return detailGameobjects.ToArray();

            if (detailMat == null)
                detailMat = new Material(Shader.Find("Diffuse"));

            detailMat.mainTexture = detailtexture;
            for (int i = 0; i < numberOfMeshes; i++)
            {
                GameObject details = new GameObject("details " + i);
                details.AddComponent<MeshFilter>().mesh = exportObject.detailMeshes[i];
                details.AddComponent<MeshRenderer>().sharedMaterial = detailMat;
                detailGameobjects.Add(details);
            }
            //        Debug.Log("Building Detail Pack Complete: " + (Time.realtimeSinceStartup - timestart) + " sec");
            return detailGameobjects.ToArray();
        }

        /// <summary>
        /// Generate the detail meshes and return the export object
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BuildingDetailExportObject Build(DynamicMeshGenericMultiMaterialMesh mesh, Data data)
        {
            BuildingDetailExportObject exportObject = new BuildingDetailExportObject();
            List<Texture2D> detailTextures = new List<Texture2D>();
            List<int> detailSubmeshesWithTextures = new List<int>();
            int numberOfDetails = data.Details.Count;
            mesh.Clear();
            mesh.subMeshCount = numberOfDetails;

            for (int d = 0; d < numberOfDetails; d++)
            {
                Detail detail = data.Details[d];
                if (detail.mesh == null)
                    continue;
                int faceIndex = detail.face;
                Vector3 position = Vector3.zero;
                Plan plan = data.Plan;
                int numberOfVolumes = plan.numberOfVolumes;
                Vector2 faceUv = detail.faceUv;
                Quaternion faceAngle = Quaternion.identity;
                if (detail.type == Detail.Types.Facade)
                {
                    //find facade
                    int facadeCount = 0;
                    bool facadeFound = false;
                    for (int s = 0; s < numberOfVolumes; s++)
                    {
                        Volume volume = plan.volumes[s];
                        int numberOfVolumePoints = volume.points.Count;
                        for (int p = 0; p < numberOfVolumePoints; p++)
                        {
                            if (facadeCount == faceIndex)
                            {
                                int indexA = p;
                                int indexB = (p + 1) % numberOfVolumePoints;
                                Vector3 p0 = plan.points[volume.points[indexA]].Vector3();
                                Vector3 p1 = plan.points[volume.points[indexB]].Vector3();
                                Vector3 basePosition = Vector3.Lerp(p0, p1, faceUv.x);
                                Vector3 detailHeight = Vector3.up * (volume.numberOfFloors * data.FloorHeight * faceUv.y);
                                Vector3 facadeCross = Vector3.Cross(Vector3.up, p1 - p0).normalized;
                                Vector3 detailDepth = facadeCross * detail.faceHeight;
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
                    Volume volume = plan.volumes[Mathf.Clamp(0, numberOfVolumes - 1, faceIndex)];
                    int numberOfVolumePoints = volume.points.Count;
                    Vector3 minimumRoofPoint = plan.points[volume.points[0]].Vector3();
                    Vector3 maximumRoofPoint = minimumRoofPoint;
                    for (int p = 1; p < numberOfVolumePoints; p++)
                    {
                        Vector3 p0 = plan.points[volume.points[p]].Vector3();
                        if (p0.x < minimumRoofPoint.x) minimumRoofPoint.x = p0.x;
                        if (p0.z < minimumRoofPoint.y) minimumRoofPoint.y = p0.z;
                        if (p0.x > maximumRoofPoint.x) maximumRoofPoint.x = p0.x;
                        if (p0.z > maximumRoofPoint.y) maximumRoofPoint.y = p0.z;
                    }
                    position.x = Mathf.Lerp(minimumRoofPoint.x, maximumRoofPoint.x, faceUv.x);
                    position.z = Mathf.Lerp(minimumRoofPoint.y, maximumRoofPoint.y, faceUv.y);
                    position.y = volume.numberOfFloors * data.FloorHeight + detail.faceHeight;
                }

                Quaternion userRotation = Quaternion.Euler(detail.userRotation);
                int vertexCount = detail.mesh.vertexCount;
                Vector3[] verts = new Vector3[vertexCount];
                Quaternion rotate = faceAngle * userRotation;
                for (int i = 0; i < vertexCount; i++)
                {
                    Vector3 sourceVertex = Vector3.Scale(detail.mesh.vertices[i], detail.scale);
                    Vector3 outputVertex = (rotate) * sourceVertex + position;
                    verts[i] = outputVertex;
                }
                mesh.AddData(verts, detail.mesh.uv, detail.mesh.triangles, d);
                detail.worldPosition = position;
                detail.worldRotation = rotate;

                if (detail.material.mainTexture != null)
                {
#if UNITY_EDITOR
                    string texturePath = AssetDatabase.GetAssetPath(detail.material.mainTexture);
                    TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(texturePath);

                    if (!textureImporter.isReadable)
                    {
                        Debug.LogWarning("The texture you have selected is not readable. Cannot render");
                        return exportObject;
                    }

                    detailTextures.Add((Texture2D)detail.material.mainTexture);
                    detailSubmeshesWithTextures.Add(d);
#endif
                }
            }

            if (detailtexture != null)
                Object.DestroyImmediate(detailtexture);

            List<Mesh> outputMeshes = new List<Mesh>();
            if (detailSubmeshesWithTextures.Count > 0)
            {
                Rect[] textureRects = BuildingTexturePacker2.Pack(out detailtexture, detailTextures.ToArray(), 512);
                if (detailSubmeshesWithTextures.Count > 0) mesh.Atlas(detailSubmeshesWithTextures.ToArray(), textureRects);
                mesh.CollapseSubmeshes();
                mesh.Build();
                int numberOfMeshes = mesh.meshCount;
                for (int i = 0; i < numberOfMeshes; i++)
                    outputMeshes.Add(mesh[i].mesh);
            }

            exportObject.detailMeshes = outputMeshes.ToArray();
            exportObject.texture = detailtexture;
            return exportObject;
            /*if (detailMat == null)
                    detailMat = new Material(Shader.Find("Diffuse"));
                detailMat.mainTexture = detailtexture;
                List<Mesh> outputMeshes = new List<Mesh>();
                for (int i = 0; i < numberOfMeshes; i++)
                {
                    outputMeshes.Add(mesh[i].mesh);
                    GameObject details = new GameObject("details " + i);
                    details.AddComponent<MeshFilter>().mesh = mesh[i].mesh;
                    details.AddComponent<MeshRenderer>().sharedMaterial = detailMat;
                    detailGameobjects.Add(details);
                }
            }
            //        Debug.Log("Building Detail Pack Complete: " + (Time.realtimeSinceStartup - timestart) + " sec");
            return detailGameobjects.ToArray();*/
        }
    }
}
