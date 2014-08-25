using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Unity;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Roads;
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
        private readonly AlphaMapGenerator _alphaMapGenerator = new AlphaMapGenerator();
        private readonly HeightMapGenerator _heightMapGenerator = new HeightMapGenerator();

        [Dependency]
        public TerrainBuilder(IRoadBuilder roadBuilder)
        {
            _roadBuilder = roadBuilder;
        }

        public IGameObject Build(IGameObject parent, TerrainSettings settings)
        {
            var size = new Vector3(settings.TerrainSize, settings.TerrainHeight, settings.TerrainSize);

            // fill heightmap
            var heightMapElements = CreateElements(settings, size, settings.Elevations);
            var htmap = _heightMapGenerator.FillHeights(settings, heightMapElements);

            // create TerrainData
            var terrainData = new TerrainData();
            terrainData.heightmapResolution = settings.HeightMapSize;
            terrainData.SetHeights(0, 0, htmap);
            terrainData.size = size;
            terrainData.splatPrototypes = GetSplatPrototypes(settings.TextureParams);

            // fill alphamap
            var alphaMapElements = CreateElements(settings, size, settings.Areas);
            var alphamap = _alphaMapGenerator.GetAlphaMap(settings, alphaMapElements);

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
                var style = settings.RoadStyleProvider.Get(road);
                _roadBuilder.Build(road, style);
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
                // TODO remove hardcoded path
                // NOTE use TerrainSettings and mapcss rule?
                splatPrototype.texture = Resources.Load<Texture2D>(@"Textures/Terrain/" + texture[1].Trim());
                splatPrototype.tileSize = new Vector2(int.Parse(texture[2]), int.Parse(texture[3]));

                splatPrototypes[i] = splatPrototype;
            }
            return splatPrototypes;
        }

        private TerrainElement[] CreateElements(TerrainSettings settings, 
            Vector3 size, IEnumerable<AreaSettings> areas)
        {
            var widthRatio = settings.AlphaMapSize / size.x;
            var heightRatio = settings.AlphaMapSize / size.z;

            var elements = areas.Select(a => new TerrainElement()
            {
                ZIndex = a.ZIndex,
                SplatIndex = a.SplatIndex,
                Points = a.Points.Select(p =>
                    ConvertWorldToTerrain(p.X, p.Y, settings.CenterPosition, widthRatio, heightRatio)).ToArray()
            }).ToArray();

            return elements.OrderBy(p => p.SplatIndex).ToArray();
        }

        private static Vector2 ConvertWorldToTerrain(float x, float y, Vector2 terrainPosition, float widthRatio, float heightRatio)
        {
            return new Vector2
            {
                // NOTE Coords are inverted here!
                y = (x - terrainPosition.x) * widthRatio,
                x = (y - terrainPosition.y) * heightRatio
            };
        } 
    }
}