using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Unity;
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
    public class TerrainBuilder: ITerrainBuilder
    {
        private readonly AlphaMapGenerator _alphaMapGenerator = new AlphaMapGenerator();
        private readonly HeightMapProcessor _heightMapProcessor = new HeightMapProcessor();

        public IGameObject Build(IGameObject parent, TerrainSettings settings)
        {
            ProcessTerrainObjects(settings);

            var htmap = settings.Tile.HeightMap.Data;

            // normalize
            for (int i = 0; i < settings.Tile.HeightMap.Resolution; i++)
                for (int j = 0; j < settings.Tile.HeightMap.Resolution; j++)
                    htmap[i, j] /= settings.Tile.HeightMap.MaxElevation;

            return CreateTerrainGameObject(parent, settings, htmap);
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

        protected virtual IGameObject CreateTerrainGameObject(IGameObject parent, TerrainSettings settings, float[,] htmap)
        {
            var size = new Vector3(settings.Tile.Size, settings.Tile.HeightMap.MaxElevation, settings.Tile.Size);
            // create TerrainData
            var terrainData = new TerrainData();
            terrainData.heightmapResolution = settings.Tile.HeightMap.Resolution;
            terrainData.SetHeights(0, 0, htmap);
            terrainData.size = size;
            terrainData.splatPrototypes = GetSplatPrototypes(settings.TextureParams);

            // fill alphamap
            var alphaMapElements = CreateElements(settings, settings.Areas,
                settings.AlphaMapSize / size.x,
                settings.AlphaMapSize / size.z,
                t => t.SplatIndex);
            var alphamap = _alphaMapGenerator.GetAlphaMap(settings, alphaMapElements);

            // create Terrain using terrain data
            var gameObject = UnityEngine.Terrain.CreateTerrainGameObject(terrainData);
            gameObject.transform.parent = parent.GetComponent<GameObject>().transform;
            var terrain = gameObject.GetComponent<UnityEngine.Terrain>();

            terrain.transform.position = new Vector3(settings.CornerPosition.x, settings.ZIndex, settings.CornerPosition.y);
            terrain.heightmapPixelError = settings.PixelMapError;
            terrain.basemapDistance = settings.BaseMapDist;

            //disable this for better frame rate
            terrain.castShadows = false;

            terrainData.SetAlphamaps(0, 0, alphamap);

            terrainData.treePrototypes = GetTreePrototypes();
            SetTrees(settings, terrain, size);

            return new GameObjectWrapper("terrain", gameObject);
        }

        protected SplatPrototype[] GetSplatPrototypes(List<List<string>> textureParams)
        {
            var splatPrototypes = new SplatPrototype[textureParams.Count];
            for (int i = 0; i < textureParams.Count; i++)
            {
                var texture = textureParams[i];
                var splatPrototype = new SplatPrototype();
                // TODO remove hardcoded path
                // NOTE use TerrainSettings and mapcss rule?
                splatPrototype.texture = Resources.Load<Texture2D>(@"Textures/Terrain/" + texture[1].Trim());
                splatPrototype.tileSize = new Vector2(int.Parse(texture[2]), int.Parse(texture[3]));

                splatPrototypes[i] = splatPrototype;
            }
            return splatPrototypes;
        }

        protected TreePrototype[] GetTreePrototypes()
        {
            // TODO make this configurable
            var treeProtoTypes = new TreePrototype[3];

            treeProtoTypes[0] = new TreePrototype();
            treeProtoTypes[0].prefab = Resources.Load<GameObject>(@"Models/Trees/Alder");

            treeProtoTypes[1] = new TreePrototype();
            treeProtoTypes[1].prefab = Resources.Load<GameObject>(@"Models/Trees/Banyan");

            treeProtoTypes[2] = new TreePrototype();
            treeProtoTypes[2].prefab = Resources.Load<GameObject>(@"Models/Trees/Mimosa");

            return treeProtoTypes;
        }

        protected void SetTrees(TerrainSettings settings, UnityEngine.Terrain terrain, Vector3 size)
        {
            // set trees
            foreach (var treeDetail in settings.Trees)
            {
                TreeInstance temp = new TreeInstance();
                temp.position = new Vector3(
                    (treeDetail.Point.X - settings.CornerPosition.x) / size.x,
                    1,
                    (treeDetail.Point.Y - settings.CornerPosition.y) / size.z);

                temp.prototypeIndex = UnityEngine.Random.Range(0, 3);
                temp.widthScale = 1;
                temp.heightScale = 1;
                temp.color = Color.white;
                temp.lightmapColor = Color.white;

                terrain.AddTreeInstance(temp);
            }
        }

        private TerrainElement[] CreateElements(TerrainSettings settings,
            IEnumerable<AreaSettings> areas, float widthRatio, float heightRatio, Func<TerrainElement, float> orderBy)
        {
            return areas.Select(a => new TerrainElement()
            {
                ZIndex = a.ZIndex,
                SplatIndex = a.SplatIndex,
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