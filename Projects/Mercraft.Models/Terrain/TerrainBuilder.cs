using System.Collections.Generic;
using Mercraft.Core.Unity;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Roads;
using Mercraft.Models.Unity;
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
        private readonly IRoadBuilder _roadBuilder;

        [Dependency]
        public TerrainBuilder(IRoadBuilder roadBuilder)
        {
            _roadBuilder = roadBuilder;
        }

        public IGameObject Build(IGameObject parent, TerrainSettings settings)
        {
            //var heightMapGenerator = new HeightMapGenerator(settings);
            var alphaMapGenerator = new AlphaMapGenerator(settings);

            // fill heightmap
            var htmap = new float[settings.HeightMapSize, settings.HeightMapSize];
            // NOTE do not use heightmap generator so far as we assume that map is flat
            //_heightMapGenerator.FillHeights(htmap);

            // create TerrainData
            var terrainData = new TerrainData();
            terrainData.heightmapResolution = settings.HeightMapSize;
            terrainData.SetHeights(0, 0, htmap);
            terrainData.size = new Vector3(settings.TerrainSize, settings.TerrainHeight, settings.TerrainSize);
            terrainData.splatPrototypes = GetSplatPrototypes(settings.TextureParams);

            // fill alphamap
            var alphamap =alphaMapGenerator.GetAlphaMap(new UnityTerrainData(terrainData));

            // create Terrain using terrain data
            var gameObject = UnityEngine.Terrain.CreateTerrainGameObject(terrainData);
            gameObject.transform.parent = parent.GetComponent<GameObject>().transform;
            var terrain = gameObject.GetComponent<UnityEngine.Terrain>();

            terrain.transform.position = new Vector3(settings.CornerPosition.x, 0, settings.CornerPosition.y);
            terrain.heightmapPixelError = settings.PixelMapError;
            terrain.basemapDistance = settings.BaseMapDist;

            //disable this for better frame rate
            terrain.castShadows = false;

            var terrainGameObject = new GameObjectWrapper("terrain", gameObject);

            // process roads
            foreach (var road in settings.Roads)
            {
                _roadBuilder.Build(road);
            }

            terrainData.SetAlphamaps(0, 0, alphamap);

            return terrainGameObject;
        }

        protected SplatPrototype[] GetSplatPrototypes(List<List<string>> textureParams)
        {
            var splatPrototypes = new SplatPrototype[textureParams.Count];
            for (int i = 0; i < textureParams.Count; i++)
            {
                var texture = textureParams[i];
                var splatPrototype = new SplatPrototype();
                splatPrototype.texture = Resources.Load<Texture2D>("Textures/" + texture[1].Trim());
                splatPrototype.tileSize = new Vector2(int.Parse(texture[2]), int.Parse(texture[3]));

                splatPrototypes[i] = splatPrototype;
            }
            return splatPrototypes;
        }
    }
}