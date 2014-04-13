using System.Collections.Generic;
using Mercraft.Models.Buildings.Builders;
using Mercraft.Models.Buildings.Entities;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;

namespace Mercraft.Models.Buildings
{
    public class BuildingBehavior : MonoBehaviour
    {
        public RenderMode renderMode = RenderMode.Full;

        private DynamicMeshGenericMultiMaterialMesh fullMesh = null;
        private DynamicMeshGenericMultiMaterialMesh detailMesh = null;
        private Data data;
        private List<GameObject> meshHolders = new List<GameObject>();
        private MeshFilter meshFilt = null;
        private MeshRenderer meshRend = null;
        private Material lowDetailMat = new Material(Shader.Find("Diffuse"));
        private List<Material> materials;
        private GameObject[] details;

        public void Attach(float height, int levels, IEnumerable<Vector2> footPrint)
        {
            data = new Data();
            data.Footprint = footPrint;
            
            ConstraitProvider.LoadConstraints("Assets/Resources/Buildings/Config/styles/testStyles.xml", data);

            Generate(data, height, levels);
        }

        private void Generate(Data data, float height, int levels)
        {
            BuildingGenerator.Generate(data, height, levels);

            UpdateRender(renderMode);
            //Ensure that we don't generate a building that has more than 65000 verts.
            int vertexCount = fullMesh.vertexCount;
            int it = 20;
            while (vertexCount > 65000)
            {
                float divisor = 65000.0f / vertexCount;
                divisor = Mathf.Floor(divisor * 10.0f) / 10.0f;
                divisor = Mathf.Min(divisor, 0.8f);
                int volumeNUmber = data.Plan.volumes.Count;
                for (int i = 0; i < volumeNUmber; i++)
                {
                    data.Plan.volumes[i].height *= divisor; //reduce the height of each volume
                    data.Plan.volumes[i].numberOfFloors = Mathf.RoundToInt(data.Plan.volumes[i].height / data.FloorHeight);
                }
                UpdateRender(renderMode);
                vertexCount = fullMesh.vertexCount;

                it--;
                if (it < 0)
                    break;
            }


            /*// Build Tangents
            fullMesh.SolveTangents();
            detailMesh.SolveTangents();

            // Build Lightmap UVs
            for (int i = 0; i < fullMesh.meshCount; i++)
                Unwrapping.GenerateSecondaryUVSet(fullMesh[i].mesh);
            for (int i = 0; i < detailMesh.meshCount; i++)
                Unwrapping.GenerateSecondaryUVSet(detailMesh[i].mesh);
            fullMesh.lightmapUvsCalculated = true;
            detailMesh.lightmapUvsCalculated = true;

            // Optimise Mesh For Runtime
            for (int i = 0; i < fullMesh.meshCount; i++)
                MeshUtility.Optimize(fullMesh[i].mesh);
            for (int i = 0; i < detailMesh.meshCount; i++)
                MeshUtility.Optimize(detailMesh[i].mesh);
            fullMesh.optimised = true;
            detailMesh.optimised = true;*/

            // Optimise Mesh For Runtime
           // for (int i = 0; i < fullMesh.meshCount; i++)
            //    MeshUtility.Optimize(fullMesh[i].mesh);
           // for (int i = 0; i < detailMesh.meshCount; i++)
           //     MeshUtility.Optimize(detailMesh[i].mesh);
        }


        private void UpdateRender(RenderMode mode)
        {
            //  _mode = renderModes.lowDetail;
            if (data.Plan == null)
                return;
            if (data.FloorHeight == 0)
                return;
            if (fullMesh == null)
                fullMesh = new DynamicMeshGenericMultiMaterialMesh();

            fullMesh.Clear();
            fullMesh.subMeshCount = data.Textures.Count;

            switch (mode)
            {
                case RenderMode.Full:
                    FullDetailBuilder.Build(fullMesh, data);
                    Roof.Build(fullMesh, data);
                    break;

                case RenderMode.LowDetail:
                    LowDetailBuilder.Build(fullMesh, data);
                    fullMesh.CollapseSubmeshes();
                    break;

                case RenderMode.Box:
                    BuildingBoxBuilder.Build(fullMesh, data);
                    break;
            }

            fullMesh.Build(false);

            while (meshHolders.Count > 0)
            {
                GameObject destroyOld = meshHolders[0];
                meshHolders.RemoveAt(0);
                DestroyImmediate(destroyOld);
            }

            int numberOfMeshes = fullMesh.meshCount;
            for (int i = 0; i < numberOfMeshes; i++)
            {
                GameObject newMeshHolder = new GameObject("model " + (i + 1));
                newMeshHolder.transform.parent = transform;
                newMeshHolder.transform.localPosition = Vector3.zero;
                meshFilt = newMeshHolder.AddComponent<MeshFilter>();
                meshRend = newMeshHolder.AddComponent<MeshRenderer>();
                meshFilt.mesh = fullMesh[i].mesh;
                meshHolders.Add(newMeshHolder);
            }

            switch (mode)
            {
                case RenderMode.Full:
                    renderMode = RenderMode.Full;
                    UpdateTextures();
                    UpdateDetails();
                    break;

                case RenderMode.LowDetail:
                    renderMode = RenderMode.LowDetail;
                    meshRend.sharedMaterials = new Material[0];
                    lowDetailMat.mainTexture = data.LodTextureAtlas;
                    meshRend.sharedMaterial = lowDetailMat;
                    UpdateDetails();
                    break;

                case RenderMode.Box:
                    renderMode = RenderMode.Box;
                    meshRend.sharedMaterials = new Material[0];
                    lowDetailMat.mainTexture = data.Textures[0].texture;
                    meshRend.sharedMaterial = lowDetailMat;
                    UpdateDetails();
                    break;
            }

        }

        private void UpdateTextures()
        {
            int numberOfMaterials = data.Textures.Count;
            if (materials == null)
                materials = new List<Material>(numberOfMaterials);
            materials.Clear();
            for (int m = 0; m < numberOfMaterials; m++)
            {
                materials.Add(data.Textures[m].material);
                materials[m].name = data.Textures[m].name;
                materials[m].mainTexture = data.Textures[m].texture;
            }
            //meshRend.sharedMaterials = materials.ToArray();

            int numberOfMeshes = fullMesh.meshCount;
            for (int i = 0; i < numberOfMeshes; i++)
                meshHolders[i].GetComponent<MeshRenderer>().sharedMaterials = materials.ToArray();
        }

        private void UpdateDetails()
        {
            if (data.Plan == null)
                return;
            if (data.FloorHeight == 0)
                return;
            if (data.Details.Count == 0)
                return;
            if (detailMesh == null)
                detailMesh = new DynamicMeshGenericMultiMaterialMesh();

            int numberOfDetails = details.Length;
            for (int i = 0; i < numberOfDetails; i++)
                DestroyImmediate(details[i]);

            if (renderMode != RenderMode.Full)
                return;//once data is cleared - asses if we want to rerender the details

            details = BuildingDetails.Render(detailMesh, data);
            numberOfDetails = details.Length;

            for (int i = 0; i < numberOfDetails; i++)
            {
                details[i].transform.parent = transform;
            }
        }

    }
}
