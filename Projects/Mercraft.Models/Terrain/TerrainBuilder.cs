using Mercraft.Core.Unity;
using Mercraft.Models.Roads;
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
        private readonly RoadBuilder _roadBuilder;

        public TerrainBuilder(TerrainSettings settings)
        {
            _settings = settings;
            _heightMapGenerator = new HeightMapGenerator(_settings);
            _alphaMapGenerator = new AlphaMapGenerator(_settings);
            _roadBuilder = new RoadBuilder();
        }

        public IGameObject Build(IGameObject parent)
        {
            // fill heightmap
            var htmap = new float[_settings.HeightMapSize, _settings.HeightMapSize];
            // NOTE do not use heightmap generator so far as we assume that map is flat
            //_heightMapGenerator.FillHeights(htmap);

            // create TerrainData
            var terrainData = new TerrainData();
            terrainData.heightmapResolution = _settings.HeightMapSize;
            terrainData.SetHeights(0, 0, htmap);
            terrainData.size = new Vector3(_settings.TerrainSize, _settings.TerrainHeight, _settings.TerrainSize);
            terrainData.splatPrototypes = _settings.SplatPrototypes;

            // fill alphamap
            var alphamap =_alphaMapGenerator.GetAlphaMap(new UnityTerrainData(terrainData));

            // create Terrain using terrain data
            var gameObject = UnityEngine.Terrain.CreateTerrainGameObject(terrainData);
            gameObject.transform.parent = parent.GetComponent<GameObject>().transform;
            var terrain = gameObject.GetComponent<UnityEngine.Terrain>();

            terrain.transform.position = new Vector3(_settings.CornerPosition.x, 0, _settings.CornerPosition.y);
            terrain.heightmapPixelError = _settings.PixelMapError;
            terrain.basemapDistance = _settings.BaseMapDist;

            //disable this for better frame rate
            terrain.castShadows = false;

            var terrainGameObject = new GameObjectWrapper("terrain", gameObject);
            
            // process roads
            foreach (var roadSetting in _settings.Roads)
            {
                roadSetting.TerrainObject = terrainGameObject;
                _roadBuilder.Build(roadSetting);
            }

            terrainData.SetAlphamaps(0, 0, alphamap);

            return terrainGameObject;
        }
    }
}