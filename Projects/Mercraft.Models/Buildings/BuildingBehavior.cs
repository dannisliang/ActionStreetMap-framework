using System.Collections.Generic;
using Mercraft.Models.Buildings.Builders;
using Mercraft.Models.Buildings.Entities;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;

namespace Mercraft.Models.Buildings
{
    public class BuildingBehavior : MonoBehaviour
    {
        private RenderMode _renderMode;

        private DynamicMeshGenericMultiMaterialMesh _fullMesh = new DynamicMeshGenericMultiMaterialMesh();
        private DynamicMeshGenericMultiMaterialMesh _detailMesh = new DynamicMeshGenericMultiMaterialMesh();
        private Data _data;
        private List<GameObject> _meshHolders = new List<GameObject>();
        private MeshFilter _meshFilt;
        private MeshRenderer _meshRend;
        private Material _lowDetailMat = new Material(Shader.Find("Diffuse"));
        private List<Material> _materials;
        private GameObject[] _details;

        public void Attach(RenderMode mode, BuildingSettings settings)
        {
            _renderMode = mode;
            _data = new Data
            {
                Footprint = settings.FootPrint,
                Style = settings.Style,
                TexturePack = settings.TexturePack,
                RanGen = new RandomGenerator((uint)settings.Seed)
            };

            Generate(_data, settings.Height, settings.Levels);
        }

        private void Generate(Data data, float height, int levels)
        {
            BuildingGenerator.Generate(data, height, levels);

            UpdateRender(_renderMode);

            int vertexCount = _fullMesh.VertexCount;
            int it = 20;
            //ensure that we don't generate a building that has more than 65000 verts.
            while (vertexCount > 65000)
            {
                float divisor = 65000.0f / vertexCount;
                divisor = Mathf.Floor(divisor * 10.0f) / 10.0f;
                divisor = Mathf.Min(divisor, 0.8f);
                int volumeNUmber = data.Plan.Volumes.Count;
                for (int i = 0; i < volumeNUmber; i++)
                {
                    data.Plan.Volumes[i].Height *= divisor; //reduce the height of each volume
                    data.Plan.Volumes[i].NumberOfFloors = Mathf.RoundToInt(data.Plan.Volumes[i].Height / data.FloorHeight);
                }
                UpdateRender(_renderMode);
                vertexCount = _fullMesh.VertexCount;

                it--;
                if (it < 0)
                    break;
            }

            Cleanup();
        }


        private void UpdateRender(RenderMode mode)
        {
            _fullMesh.SubMeshCount = _data.Textures.Count;

            var roofBuilder = new RoofBuilder(_data, _fullMesh);

            switch (mode)
            {
                case RenderMode.Full:
                    FullDetailBuilder.Build(_fullMesh, _data);
                    roofBuilder.Build();
                    break;

                case RenderMode.Low:
                    LowDetailBuilder.Build(_fullMesh, _data);
                    _fullMesh.CollapseSubmeshes();
                    break;

                case RenderMode.Box:
                    BuildingBoxBuilder.Build(_fullMesh, _data);
                    roofBuilder.Build();
                    break;
            }

            _fullMesh.Build(false);

            while (_meshHolders.Count > 0)
            {
                GameObject destroyOld = _meshHolders[0];
                _meshHolders.RemoveAt(0);
                DestroyImmediate(destroyOld);
            }

            int numberOfMeshes = _fullMesh.MeshCount;
            for (int i = 0; i < numberOfMeshes; i++)
            {
                GameObject newMeshHolder = new GameObject("model " + (i + 1));
                newMeshHolder.transform.parent = transform;
                newMeshHolder.transform.localPosition = Vector3.zero;
                _meshFilt = newMeshHolder.AddComponent<MeshFilter>();
                _meshRend = newMeshHolder.AddComponent<MeshRenderer>();
                _meshFilt.mesh = _fullMesh[i].Mesh;
                _meshHolders.Add(newMeshHolder);
            }

            switch (mode)
            {
                case RenderMode.Full:
                    _renderMode = RenderMode.Full;
                    UpdateTextures();
                    UpdateDetails();
                    break;

                case RenderMode.Low:
                    _renderMode = RenderMode.Low;
                    _meshRend.sharedMaterials = new Material[0];
                    _lowDetailMat.mainTexture = _data.LodTextureAtlas;
                    _meshRend.sharedMaterial = _lowDetailMat;
                    UpdateDetails();
                    break;

                case RenderMode.Box:
                    _renderMode = RenderMode.Box;
                    _meshRend.sharedMaterials = new Material[0];
                    _lowDetailMat.mainTexture = _data.Textures[0].MainTexture;
                    _meshRend.sharedMaterial = _lowDetailMat;
                    UpdateDetails();
                    break;
            }

        }

        private void UpdateTextures()
        {
            int numberOfMaterials = _data.Textures.Count;
            if (_materials == null)
                _materials = new List<Material>(numberOfMaterials);
            _materials.Clear();
            for (int m = 0; m < numberOfMaterials; m++)
            {
                _materials.Add(_data.Textures[m].Material);
                _materials[m].name = _data.Textures[m].Name;
                _materials[m].mainTexture = _data.Textures[m].MainTexture;
            }

            int numberOfMeshes = _fullMesh.MeshCount;
            for (int i = 0; i < numberOfMeshes; i++)
                _meshHolders[i].GetComponent<MeshRenderer>().sharedMaterials = _materials.ToArray();
        }

        private void UpdateDetails()
        {
            if (_data.Details.Count == 0)
                return;

            int numberOfDetails = _details.Length;
            for (int i = 0; i < numberOfDetails; i++)
                DestroyImmediate(_details[i]);

            if (_renderMode != RenderMode.Full)
                return;

            _details = DetailsBuilder.Render(_detailMesh, _data);
            numberOfDetails = _details.Length;

            for (int i = 0; i < numberOfDetails; i++)
            {
                _details[i].transform.parent = transform;
            }
        }

        public void Cleanup()
        {
            _fullMesh = null;
            _detailMesh = null;
            _data = null;
            _meshRend = null;
            _meshFilt = null;
            _meshHolders.Clear();
            _lowDetailMat = null;
            if (_materials != null)
                _materials.Clear();
            _details = null;
        }
    }
}
