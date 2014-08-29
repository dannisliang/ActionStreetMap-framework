using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.Elevation;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Core.World.Roads;
using Mercraft.Explorer.Helpers;
using Mercraft.Explorer.Themes;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm.Helpers;
using Mercraft.Models.Roads;
using Mercraft.Models.Terrain;
using UnityEngine;

namespace Mercraft.Explorer
{
    public class SceneVisitor : ISceneVisitor
    {
        private readonly IGameObjectFactory _goFactory;
        private readonly IThemeProvider _themeProvider;
        private readonly ITerrainBuilder _terrainBuilder;
        private readonly IRoadBuilder _roadBuilder;
        private readonly IHeightMapProvider _heightMapProvider;
        private readonly IEnumerable<IModelBuilder> _builders;
        private readonly IEnumerable<IModelBehaviour> _behaviours;

        // TODO refactoring: I don't like to store this here
        private float[,] _heightMap;
        private int _heightMapResolution;
        private float _maxElevation;

        private List<AreaSettings> _areas = new List<AreaSettings>();
        private List<AreaSettings> _elevations = new List<AreaSettings>();
        private List<RoadElement> _roadElements = new List<RoadElement>();

        [Dependency]
        public SceneVisitor(IGameObjectFactory goFactory,
            IThemeProvider themeProvider,
            ITerrainBuilder terrainBuilder,
            IRoadBuilder roadBuilder,
            IHeightMapProvider heightMapProvider,
            IEnumerable<IModelBuilder> builders,
            IEnumerable<IModelBehaviour> behaviours)
        {
            _goFactory = goFactory;
            _themeProvider = themeProvider;
            _terrainBuilder = terrainBuilder;
            _roadBuilder = roadBuilder;
            _heightMapProvider = heightMapProvider;
            _builders = builders.ToList();
            _behaviours = behaviours.ToList();
        }

        #region ISceneVisitor implementation

        public void Prepare(IScene scene, Stylesheet stylesheet)
        {
            //  NOTE We have to precalculate heightmap to use it's values 
            // in all models to build non-flat world

            var tile = scene.Canvas.Tile;

            _heightMapResolution = stylesheet.GetRule(scene.Canvas, false).GetHeightMapSize();

            var center = GeoProjection.ToGeoCoordinate(tile.RelativeNullPoint, tile.TileMapCenter);

            _heightMap = _heightMapProvider.GetHeightMap(center, _heightMapResolution, tile.Size, 
                out _maxElevation);
        }

        public void Finalize(IScene scene)
        {
            // NOTE not ideal solution to make the class ready for next request
            _areas = new List<AreaSettings>();
            _elevations = new List<AreaSettings>();
            _roadElements = new List<RoadElement>();
        }

        public bool VisitCanvas(GeoCoordinate center, IGameObject parent, Rule rule, Canvas canvas, bool visitedBefore)
        {
            var tile = canvas.Tile;

            // TODO compose roads
            var roads = _roadElements.Select(re => new Road()
            {
                Elements = new List<RoadElement>() { re },
                GameObject = _goFactory.CreateNew(String.Format("road [{0}] {1}/ ", re.Id, re.Address), parent),
            }).ToArray();

            var roadStyleProvider = _themeProvider.Get()
                .GetStyleProvider<IRoadStyleProvider>();

            // process roads
            foreach (var road in roads)
            {
                var style = roadStyleProvider.Get(road);
                _roadBuilder.Build(road, style);
            }

            _terrainBuilder.Build(parent, new TerrainSettings()
            {
                AlphaMapSize = rule.GetAlphaMapSize(),
                HeightMapSize = _heightMapResolution,
                CenterPosition = new Vector2(tile.TileMapCenter.X, tile.TileMapCenter.Y),
                TerrainSize = tile.Size,
                TerrainHeight = rule.GetHeight(),
                ZIndex = rule.GetZIndex(),
                TextureParams = rule.GetTextureParams(),
                Areas = _areas,
                Elevations = _elevations
            });
            return true;
        }

        public bool VisitArea(GeoCoordinate center, IGameObject parent, Rule rule, Area area, bool visitedBefore)
        {
            if (rule.IsSkipped())
            {
                CreateSkipped(parent, area);
                return true;
            }

            var builder = rule.GetModelBuilder(_builders);
            bool processed = false;
            if (rule.IsTerrain())
            {
                _areas.Add(new AreaSettings()
                {
                    ZIndex = rule.GetZIndex(),
                    SplatIndex = rule.GetSplatIndex(),
                    Points = PolygonHelper.GetVerticies2D(center, area.Points)
                });
                // TODO in future we want to build some special object which will be
                // invisible as it's part of terrain but useful to provide some OSM info
                // which is associated with it
                processed = true;
            }

            if (rule.IsElevation())
            {
                _elevations.Add(new AreaSettings()
                {
                    ZIndex = rule.GetZIndex(),
                    Points = PolygonHelper.GetVerticies2D(center, area.Points)
                });
                processed = true;
            }

            if (builder == null)
            {
                if (processed)
                    return true;

                // mapcss rule should contain builder
                throw new InvalidOperationException(String.Format("Incorrect mapcss rule for {0}", area));
            }

            if (!visitedBefore)
            {
                var gameObjectWrapper = builder.BuildArea(center, rule, area);
                gameObjectWrapper.Name = String.Format("{0} {1}", builder.Name, area);
                gameObjectWrapper.Parent = parent;

                ApplyBehaviour(gameObjectWrapper, rule, area);
            }

            return true;
        }

        public bool VisitWay(GeoCoordinate center, IGameObject parent, Rule rule, Way way, bool visitedBefore)
        {
            // mapcss rule is set to skip this element
            if (rule.IsSkipped())
            {
                CreateSkipped(parent, way);
                // return true to mark this element as processed
                return true;
            }

            bool processed = false;
            var builder = rule.GetModelBuilder(_builders);
            // mapcss rule is set to road
            if (rule.IsRoad())
            {
                _roadElements.Add(new RoadElement()
                {
                    Id = way.Id,
                    Address = AddressExtractor.Extract(way.Tags),
                    Width = (int)Math.Round(rule.GetWidth() / 2),
                    ZIndex = rule.GetZIndex(),
                    Points = way.Points.Select(p => GeoProjection.ToMapCoordinate(center, p)).ToArray()
                });

                processed = true;
            }

            if (builder == null)
            {
                if (processed)
                    return true;
                // mapcss rule should contain builder
                throw new InvalidOperationException(String.Format("Incorrect mapcss rule for {0}", way));
            }               

            var gameObjectWrapper = builder.BuildWay(center, rule, way);
            gameObjectWrapper.Name = String.Format("{0} {1}", builder.Name, way);
            gameObjectWrapper.Parent = parent;

            ApplyBehaviour(gameObjectWrapper, rule, way);

            return true;
        }

        #endregion

        private void CreateSkipped(IGameObject parent, Model model)
        {
            // TODO 
            var skippedGameObject = _goFactory.CreateNew(String.Format("skip {0}", model));
            skippedGameObject.Parent = parent;
        }

        private void ApplyBehaviour(IGameObject target, Rule rule, Model model)
        {
            var behaviour = rule.GetModelBehaviour(_behaviours);
            if (behaviour != null)
                behaviour.Apply(target, model);
        }
    }
}