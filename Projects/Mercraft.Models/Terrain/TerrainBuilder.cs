using Mercraft.Core.Unity;
using Mercraft.Models.Unity;
using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Models.Terrain
{
    /// <summary>
    ///     Creates Terrain object using given settings
    /// </summary>
    public class TerrainBuilder
    {
        private readonly TerrainSettings _settings;
        private readonly HeightMapGenerator _heightMapGenerator;
        private readonly AlphaMapGenerator _alphaMapGenerator;

        public TerrainBuilder(TerrainSettings settings)
        {
            _settings = settings;
            _heightMapGenerator = new HeightMapGenerator(_settings);
            _alphaMapGenerator = new AlphaMapGenerator(_settings);
        }

        public IGameObject Build(IGameObject parent)
        {
            var htmap = new float[_settings.HeightMapSize, _settings.HeightMapSize];
            _heightMapGenerator.FillHeights(htmap);

            // create TerrainData
            var terrainData = new TerrainData();
            terrainData.heightmapResolution = _settings.HeightMapSize;
            terrainData.SetHeights(0, 0, htmap);
            terrainData.size = new Vector3(_settings.TerrainSize, _settings.TerrainHeight, _settings.TerrainSize);
            terrainData.splatPrototypes = _settings.SplatPrototypes;

            _alphaMapGenerator.FillAlphaMap(new UnityTerrainData(terrainData));

            // create Terrain
            var gameObject = UnityEngine.Terrain.CreateTerrainGameObject(terrainData);
            gameObject.transform.parent = parent.GetComponent<GameObject>().transform;
            var terrain = gameObject.GetComponent<UnityEngine.Terrain>();

            terrain.transform.position = new Vector3(_settings.CornerPosition.x, 0, _settings.CornerPosition.y);
            terrain.heightmapPixelError = _settings.PixelMapError;
            terrain.basemapDistance = _settings.BaseMapDist;

            //disable this for better frame rate
            terrain.castShadows = false;

            return new GameObjectWrapper("terrain", gameObject);
        }
    }
}