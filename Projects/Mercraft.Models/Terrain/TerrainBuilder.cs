using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Unity;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;
using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Models.Terrain
{
    public interface ITerrainBuilder
    {
        IGameObject Build(IGameObject parent, TerrainSettings settings);
    }

    /// <summary>
    ///     Creates Terrain object using given settings
    /// </summary>
    public class TerrainBuilder : ITerrainBuilder
    {
        private readonly IResourceProvider _resourceProvider;
        private readonly AreaBuilder _areaBuilder = new AreaBuilder();
        private readonly HeightMapProcessor _heightMapProcessor = new HeightMapProcessor();

        [Dependency]
        private ITrace Trace { get; set; }

        [Dependency]
        public TerrainBuilder(IResourceProvider resourceProvider)
        {
            _resourceProvider = resourceProvider;
        }

        public IGameObject Build(IGameObject parent, TerrainSettings settings)
        {
            ProcessTerrainObjects(settings);

            var htmap = settings.Tile.HeightMap.Data;

            // normalize
            for (int i = 0; i < settings.Tile.HeightMap.Resolution; i++)
                for (int j = 0; j < settings.Tile.HeightMap.Resolution; j++)
                    htmap[i, j] /= settings.Tile.HeightMap.MaxElevation;

            var size = new Vector3(settings.Tile.Size, settings.Tile.HeightMap.MaxElevation, settings.Tile.Size);
            var layers = settings.SplatParams.Count;
            var splatMap = new float[settings.Resolution, settings.Resolution, layers];
            var detailMapList = new List<int[,]>(settings.DetailParams.Count);
            // fill alphamap
            var alphaMapElements = CreateElements(settings, settings.Areas,
                settings.Resolution / size.x,
                settings.Resolution / size.z,
                t => t.SplatIndex);

            _areaBuilder.Build(settings, alphaMapElements, splatMap, detailMapList);
            return CreateTerrainGameObject(parent, settings, size, htmap, splatMap, detailMapList);
        }

        private void ProcessTerrainObjects(TerrainSettings settings)
        {
            var heightMap = settings.Tile.HeightMap;
            var roadStyleProvider = settings.RoadStyleProvider;
            var roadBuilder = settings.RoadBuilder;

            // process roads
            foreach (var road in settings.Roads)
            {
                var style = roadStyleProvider.Get(road);
                roadBuilder.Build(heightMap, road, style);
            }

            // process elevations
            if (settings.Elevations.Any())
            {
                var elevation = heightMap.MinElevation - 10;
                foreach (var elevationArea in settings.Elevations)
                {
                    _heightMapProcessor.Recycle(heightMap);
                    _heightMapProcessor.AdjustPolygon(elevationArea.Points, elevation);
                }
            }
        }

        protected virtual IGameObject CreateTerrainGameObject(IGameObject parent, TerrainSettings settings,
            Vector3 size, float[,] htmap, float[, ,] splatMap, List<int[,]> detailMapList)
        {
            // create TerrainData
            var terrainData = new TerrainData();
            terrainData.heightmapResolution = settings.Tile.HeightMap.Resolution;
            terrainData.SetHeights(0, 0, htmap);
            terrainData.size = size;
            terrainData.splatPrototypes = GetSplatPrototypes(settings.SplatParams);
            terrainData.detailPrototypes = GetDetailPrototype(settings.DetailParams);

            // create Terrain using terrain data
            var gameObject = UnityEngine.Terrain.CreateTerrainGameObject(terrainData);
            gameObject.transform.parent = parent.GetComponent<GameObject>().transform;
            var terrain = gameObject.GetComponent<UnityEngine.Terrain>();

            terrain.transform.position = new Vector3(settings.CornerPosition.x,
                settings.ZIndex, settings.CornerPosition.y);
            terrain.heightmapPixelError = settings.PixelMapError;
            terrain.basemapDistance = settings.BaseMapDist;

            //disable this for better frame rate
            terrain.castShadows = false;

            terrainData.SetAlphamaps(0, 0, splatMap);

            SetTrees(terrain, settings, size);

            SetDetails(terrain, settings, detailMapList);

            parent.GetComponent<GameObject>().AddComponent<TerrainBehaviour>().Trace = Trace;

            return new GameObjectWrapper("terrain", gameObject);
        }

        #region Alpha map splats
        protected SplatPrototype[] GetSplatPrototypes(List<List<string>> splatParams)
        {
            var splatPrototypes = new SplatPrototype[splatParams.Count];
            for (int i = 0; i < splatParams.Count; i++)
            {
                var splat = splatParams[i];
                var splatPrototype = new SplatPrototype();
                // TODO remove hardcoded path
                splatPrototype.texture = _resourceProvider.GetTexture2D(@"Textures/Terrain/" + splat[1].Trim());
                splatPrototype.tileSize = new Vector2(int.Parse(splat[2]), int.Parse(splat[3]));

                splatPrototypes[i] = splatPrototype;
            }
            return splatPrototypes;
        }

        #endregion

        #region Trees
        protected TreePrototype[] GetTreePrototypes()
        {
            // TODO make this configurable
            var treeProtoTypes = new TreePrototype[3];

            treeProtoTypes[0] = new TreePrototype();
            treeProtoTypes[0].prefab = _resourceProvider.GetGameObject(@"Models/Trees/Alder");

            treeProtoTypes[1] = new TreePrototype();
            treeProtoTypes[1].prefab = _resourceProvider.GetGameObject(@"Models/Trees/Banyan");

            treeProtoTypes[2] = new TreePrototype();
            treeProtoTypes[2].prefab = _resourceProvider.GetGameObject(@"Models/Trees/Mimosa");

            return treeProtoTypes;
        }

        protected void SetTrees(UnityEngine.Terrain terrain, TerrainSettings settings, Vector3 size)
        {
            terrain.terrainData.treePrototypes = GetTreePrototypes();
            foreach (var treeDetail in settings.Trees)
            {
                var position = new Vector3((treeDetail.Point.X - settings.CornerPosition.x) / size.x, 1,
                    (treeDetail.Point.Y - settings.CornerPosition.y) / size.z);

                // TODO investigate, why we get nodes out of current bbox for trees
                // probably, it's better to filter them in osm level (however, they should be filtered!)
                if (position.x > 1 || position.x < 0 || position.z > 1 || position.z < 0)
                    continue;

                TreeInstance temp = new TreeInstance();
                temp.position = position;
                temp.prototypeIndex = UnityEngine.Random.Range(0, 3);
                temp.widthScale = 1;
                temp.heightScale = 1;
                temp.color = Color.white;
                temp.lightmapColor = Color.white;

                terrain.AddTreeInstance(temp);
            }
        }
        #endregion

        #region Details
        protected DetailPrototype[] GetDetailPrototype(List<List<string>> detailParams)
        {
            // TODO make this configurable
            DetailRenderMode detailMode = DetailRenderMode.GrassBillboard;
            Color grassHealthyColor = Color.white;
            Color grassDryColor = Color.white;

            var detailProtoTypes = new DetailPrototype[detailParams.Count];
            for (int i = 0; i < detailParams.Count; i++)
            {
                var detail = detailParams[i];
                detailProtoTypes[i] = new DetailPrototype();
                // TODO remove hardcoded path
                detailProtoTypes[i].prototypeTexture = _resourceProvider.GetTexture2D(@"Textures/Terrain/" + detail[1].Trim());
                detailProtoTypes[i].renderMode = detailMode;
                detailProtoTypes[i].healthyColor = grassHealthyColor;
                detailProtoTypes[i].dryColor = grassDryColor;
            }

            return detailProtoTypes;
        }

        protected void SetDetails(UnityEngine.Terrain terrain, TerrainSettings settings, List<int[,]> detailMapList)
        {
            // TODO make this configurable
            int detailMapSize = settings.Resolution; //Resolutions of detail (Grass) layers
            int detailObjectDistance = 400;   //The distance at which details will no longer be drawn
            float detailObjectDensity = 1f; //Creates more dense details within patch
            int detailResolutionPerPatch = 128; //The size of detail patch. A higher number may reduce draw calls as details will be batch in larger patches
            float wavingGrassStrength = 0.4f;
            float wavingGrassAmount = 0.2f;
            float wavingGrassSpeed = 0.4f;
            Color wavingGrassTint = Color.white;

            terrain.terrainData.wavingGrassStrength = wavingGrassStrength;
            terrain.terrainData.wavingGrassAmount = wavingGrassAmount;
            terrain.terrainData.wavingGrassSpeed = wavingGrassSpeed;
            terrain.terrainData.wavingGrassTint = wavingGrassTint;
            terrain.detailObjectDensity = detailObjectDensity;
            terrain.detailObjectDistance = detailObjectDistance;
            terrain.terrainData.SetDetailResolution(detailMapSize, detailResolutionPerPatch);

            for (int i = 0; i < detailMapList.Count; i++)
                terrain.terrainData.SetDetailLayer(0, 0, i, detailMapList[i]);
            
        }
        #endregion

        private TerrainElement[] CreateElements(TerrainSettings settings,
            IEnumerable<AreaSettings> areas, float widthRatio, float heightRatio, Func<TerrainElement, float> orderBy)
        {
            return areas.Select(a => new TerrainElement()
            {
                ZIndex = a.ZIndex,
                SplatIndex = a.SplatIndex,
                DetailIndex = a.DetailIndex,
                Points = a.Points.Select(p =>
                    ConvertWorldToTerrain(p.X, p.Elevation, p.Y, settings.CornerPosition, widthRatio, heightRatio)).ToArray()
            }).OrderBy(orderBy).ToArray();
        }

        private static Vector3 ConvertWorldToTerrain(float x, float y, float z, Vector2 terrainPosition, float widthRatio, float heightRatio)
        {
            return new Vector3
            {
                // NOTE Coords are inverted here!
                z = (x - terrainPosition.x) * widthRatio,
                x = (z - terrainPosition.y) * heightRatio,
                y = y,
            };
        }
    }
}