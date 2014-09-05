using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.Elevation;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Core.World.Roads;
using Mercraft.Explorer.Builders;
using Mercraft.Explorer.Helpers;
using Mercraft.Explorer.Themes;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm.Helpers;
using Mercraft.Models.Roads;
using Mercraft.Models.Terrain;
using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Explorer
{
    // TODO Refactoring this class is overcomplicated
    public class SceneVisitor : ISceneVisitor
    {
        private readonly IGameObjectFactory _goFactory;
        private readonly IThemeProvider _themeProvider;
        
        private readonly ITerrainBuilder _terrainBuilder;
        private readonly IRoadBuilder _roadBuilder;

        private readonly IHeightMapProvider _heightMapProvider;
        private readonly IEnumerable<IModelBuilder> _builders;
        private readonly IEnumerable<IModelBehaviour> _behaviours;

        private readonly HeightMapProcessor _heightMapProcessor = new HeightMapProcessor();

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
            // NOTE We have to precalculate heightmap to use it's values 
            // in all models to build non-flat world

            var tile = scene.Canvas.Tile;

            var heightMapResolution = stylesheet.GetRule(scene.Canvas, false).GetHeightMapSize();

            tile.HeightMap = _heightMapProvider.GetHeightMap(tile, heightMapResolution);
        }

        public void Finalize(IScene scene)
        {
            // NOTE not ideal solution to make the class ready for next request
            _areas = new List<AreaSettings>();
            _elevations = new List<AreaSettings>();
            _roadElements = new List<RoadElement>();
        }

        public SceneVisitResult VisitCanvas(Tile tile, Rule rule, Canvas canvas, bool visitedBefore)
        {
            // TODO compose roads
            var roads = _roadElements.Select(re => new Road()
            {
                Elements = new List<RoadElement>() { re },
                GameObject = _goFactory.CreateNew(String.Format("road [{0}] {1}/ ", re.Id, re.Address), tile.GameObject),
            }).ToArray();

            var roadStyleProvider = _themeProvider.Get()
                .GetStyleProvider<IRoadStyleProvider>();

            // process roads
            foreach (var road in roads)
            {
                var style = roadStyleProvider.Get(road);
                _roadBuilder.Build(tile.HeightMap, road, style);
            }

            if (_elevations.Any())
            {
                var elevation = tile.HeightMap.MinElevation - 10;
                foreach (var elevationArea in _elevations)
                {
                    _heightMapProcessor.Recycle(tile.HeightMap);
                    _heightMapProcessor.AdjustPolygon(elevationArea.Points, elevation);
                }
            }

            if (tile.HeightMap.IsFlat)
                tile.HeightMap.MaxElevation = rule.GetHeight();

            _terrainBuilder.Build(tile.GameObject, new TerrainSettings()
            {
                Tile = tile,
                AlphaMapSize = rule.GetAlphaMapSize(),
                CenterPosition = new Vector2(tile.MapCenter.X, tile.MapCenter.Y),
                CornerPosition = new Vector2(tile.BottomLeft.X, tile.BottomLeft.Y),
                PixelMapError = rule.GetPixelMapError(),
                ZIndex = rule.GetZIndex(),
                TextureParams = rule.GetTextureParams(),
                Areas = _areas,
            });

            return SceneVisitResult.Completed;
        }

        public SceneVisitResult VisitArea(Tile tile, Rule rule, Area area, bool visitedBefore)
        {
            if (rule.IsSkipped())
            {
                CreateSkipped(tile.GameObject, area);
                // mark it as completed to filter it in future
                return SceneVisitResult.Completed;
            }

            if (rule.IsTerrain())
            {
                _areas.Add(new AreaSettings()
                {
                    ZIndex = rule.GetZIndex(),
                    SplatIndex = rule.GetSplatIndex(),
                    Points = PolygonHelper.GetVerticies2D(tile.RelativeNullPoint, area.Points)
                });
                // TODO in future we want to build some special object which will be
                // invisible as it's part of terrain but useful to provide some OSM info
                // which is associated with it
                return SceneVisitResult.Partial;
            }

            if (rule.IsElevation())
            {
                _elevations.Add(new AreaSettings()
                {
                    ZIndex = rule.GetZIndex(),
                    Points = PolygonHelper.GetVerticies2D(tile.RelativeNullPoint, area.Points)
                });
            }

            var builder = rule.GetModelBuilder(_builders);
            if (builder == null)
                return SceneVisitResult.None;
            

            if (!visitedBefore)
            {
                var gameObjectWrapper = builder.BuildArea(tile, rule, area);
                gameObjectWrapper.Name = String.Format("{0} {1}", builder.Name, area);
                gameObjectWrapper.Parent = tile.GameObject;

                ApplyBehaviour(gameObjectWrapper, rule, area);
            }

            return SceneVisitResult.Completed;
        }

        public SceneVisitResult VisitWay(Tile tile, Rule rule, Way way, bool visitedBefore)
        {
            // mapcss rule is set to skip this element
            if (rule.IsSkipped())
            {
                CreateSkipped(tile.GameObject, way);
                // however we return result as completed
                return SceneVisitResult.Completed;
            }

            var builder = rule.GetModelBuilder(_builders);
            if (rule.IsRoad())
            {
                // road should be processed in canvas: it's better to collect all 
                // roads and create connected road network
                _roadElements.Add(new RoadElement()
                {
                    Id = way.Id,
                    Address = AddressExtractor.Extract(way.Tags),
                    Width = (int)Math.Round(rule.GetWidth() / 2),
                    ZIndex = rule.GetZIndex(),
                    Points = way.Points.Select(p =>
                    {
                        var mapPoint = GeoProjection.ToMapCoordinate(tile.RelativeNullPoint, p);
                        mapPoint.Elevation = tile.HeightMap.LookupHeight(mapPoint);
                        return mapPoint;
                    }).ToArray()
                });

                // flat road can be rendered fully for cross-tile case, but not for elevation
                return tile.HeightMap.IsFlat ? SceneVisitResult.Completed : SceneVisitResult.Partial;
            }

            // this implementation relies on builder declaration in mapcss
            // if we have no such declaration we cannot render this model
            if (builder == null)
                return SceneVisitResult.None;

            var gameObjectWrapper = builder.BuildWay(tile, rule, way);
            gameObjectWrapper.Name = String.Format("{0} {1}", builder.Name, way);
            gameObjectWrapper.Parent = tile.GameObject;

            ApplyBehaviour(gameObjectWrapper, rule, way);

            return SceneVisitResult.Completed;
        }

        #endregion

        private void CreateSkipped(IGameObject parent, Model model)
        {
            // TODO this is useful only for debug, in release we should avoid creation of 
            // additional objects due to performance reason
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