using System.Collections.Generic;
using Mercraft.Models.Buildings.Builders;
using Mercraft.Models.Buildings.Entities;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;

namespace Mercraft.Models.Buildings
{
    public class BuildingBehavior : MonoBehaviour
    {
        public void Attach(BuildingSettings settings)
        {
            var model = new Model
            {
                Footprint = settings.FootPrint,
                Style = settings.Style,
                TexturePack = settings.TexturePack,
                RanGen = new RandomGenerator((uint) settings.Seed)
            };

            Generate(model, settings.Height, settings.Levels);
        }

        private void Generate(Model model, float height, int levels)
        {
            var fullMesh = new DynamicMeshGenericMultiMaterialMesh();
            BuildingGenerator.Generate(model, height, levels);

            UpdateRender(model, fullMesh);

            int vertexCount = fullMesh.VertexCount;
            int it = 20;
            //ensure that we don't generate a building that has more than 65000 verts.
            while (vertexCount > 65000)
            {
                float divisor = 65000.0f/vertexCount;
                divisor = Mathf.Floor(divisor*10.0f)/10.0f;
                divisor = Mathf.Min(divisor, 0.8f);
                int volumeNUmber = model.Plan.Volumes.Count;
                for (int i = 0; i < volumeNUmber; i++)
                {
                    model.Plan.Volumes[i].Height *= divisor; //reduce the height of each volume
                    model.Plan.Volumes[i].NumberOfFloors =
                        Mathf.RoundToInt(model.Plan.Volumes[i].Height/model.FloorHeight);
                }
                UpdateRender(model, fullMesh);
                vertexCount = fullMesh.VertexCount;

                it--;
                if (it < 0)
                    break;
            }
        }

        private void UpdateRender(Model model, DynamicMeshGenericMultiMaterialMesh fullMesh)
        {
            var meshHolders = new List<GameObject>();
            MeshRenderer meshRenderer = null;
            var lowDetailMat = new Material(Shader.Find("Diffuse"));

            fullMesh.SubMeshCount = model.Textures.Count;

            var roofBuilder = new RoofBuilder(model, fullMesh);

            BuildingBoxBuilder.Build(fullMesh, model);
            roofBuilder.Build();

            fullMesh.Build(false);

            while (meshHolders.Count > 0)
            {
                GameObject destroyOld = meshHolders[0];
                meshHolders.RemoveAt(0);
                DestroyImmediate(destroyOld);
            }

            int numberOfMeshes = fullMesh.MeshCount;
            for (int i = 0; i < numberOfMeshes; i++)
            {
                GameObject newMeshHolder = new GameObject("model " + (i + 1));
                newMeshHolder.transform.parent = transform;
                newMeshHolder.transform.localPosition = Vector3.zero;
                MeshFilter meshFilter = newMeshHolder.AddComponent<MeshFilter>();
                meshRenderer = newMeshHolder.AddComponent<MeshRenderer>();
                meshFilter.mesh = fullMesh[i].Mesh;
                meshHolders.Add(newMeshHolder);
            }

            meshRenderer.sharedMaterials = new Material[0];
            lowDetailMat.mainTexture = model.Textures[0].MainTexture;
            meshRenderer.sharedMaterial = lowDetailMat;
        }
    }
}