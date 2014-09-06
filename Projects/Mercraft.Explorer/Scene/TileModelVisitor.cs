using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Elevation;
using Mercraft.Core.MapCss;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Core.World.Roads;
using Mercraft.Explorer.Helpers;
using Mercraft.Explorer.Themes;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Roads;
using Mercraft.Models.Terrain;
using UnityEngine;

namespace Mercraft.Explorer.Scene
{
    public class TileModelVisitor : ITileVisitor, IModelVisitor
    {
        private readonly IHeightMapProvider _heighMapProvider;
        private readonly ITerrainBuilder _terrainBuilder;
        private readonly IRoadBuilder _roadBuilder;
        private readonly IEnumerable<IModelBuilder> _builders;
        private readonly IEnumerable<IModelBehaviour> _behaviours;
        private readonly IGameObjectFactory _gameObjectFactory;
        private readonly IThemeProvider _themeProvider;
        private readonly Stylesheet _stylesheet;

        private Tile _tile;

        private IModelBuilder _terrainModelBuilder;
        private IModelBuilder _objectModelBuilder;

        private readonly List<AreaSettings> _areas = new List<AreaSettings>();
        private readonly List<AreaSettings> _elevations = new List<AreaSettings>();
        private readonly List<RoadElement> _roadElements = new List<RoadElement>();

        [Dependency]
        public TileModelVisitor(IGameObjectFactory gameObjectFactory, IThemeProvider themeProvider,
            IHeightMapProvider heighMapProvider, ITerrainBuilder terrainBuilder,             
            IRoadBuilder roadBuilder, IStylesheetProvider stylesheetProvider,
            IEnumerable<IModelBuilder> builders, IEnumerable<IModelBehaviour> behaviours)
        {
            _heighMapProvider = heighMapProvider;
            _terrainBuilder = terrainBuilder;
            _roadBuilder = roadBuilder;
            _builders = builders.ToArray();
            _behaviours = behaviours.ToArray();
            _gameObjectFactory = gameObjectFactory;
            _themeProvider = themeProvider;
            _stylesheet = stylesheetProvider.Get();
        }

        #region ITileVisitor

        public void Visit(Tile tile)
        {
            _tile = tile;
            // terrain builder knows about terrain type elements like areas (e.g. parks, green zones 
            // which should be drawn using different terrain alpha map splats), elevations 
            _terrainModelBuilder = new TerrainModelBuilder(_areas, _elevations, _roadElements);
            _objectModelBuilder = new ObjectModelBuilder(_builders, _behaviours);

            var heightMapResolution = _stylesheet.GetRule(tile.Scene.Canvas, false).GetHeightMapSize();
            tile.GameObject = _gameObjectFactory.CreateNew("tile");
            tile.HeightMap = _heighMapProvider.GetHeightMap(tile, heightMapResolution);

            foreach (var area in tile.Scene.Areas)
                area.Accept(this);
            foreach (var way in tile.Scene.Ways)
                way.Accept(this);
            tile.Scene.Canvas.Accept(this);

            // clear collections to reuse
            _areas.Clear();
            _elevations.Clear();
            _roadElements.Clear();
        }

        #endregion

        #region IModelVisitor

        public void VisitArea(Area area)
        {
            var rule = _stylesheet.GetRule(area);
            if (!rule.IsApplicable)
                return;

            if (rule.IsSkipped())
                CreateSkipped(_tile.GameObject, area);
            else 
            {
                _objectModelBuilder.BuildArea(_tile, rule, area);
                _terrainModelBuilder.BuildArea(_tile, rule, area);
            }
        }

        public void VisitWay(Way way)
        {
            var rule = _stylesheet.GetRule(way);
            if (!rule.IsApplicable)
                return;

            if (rule.IsSkipped())
                CreateSkipped(_tile.GameObject, way);
            else
            {
                _objectModelBuilder.BuildWay(_tile, rule, way);
                _terrainModelBuilder.BuildWay(_tile, rule, way);
            }
        }

        public void VisitCanvas(Canvas canvas)
        {
            var rule = _stylesheet.GetRule(_tile.Scene.Canvas, false);

            // TODO this should be done by road composer
            var roads = _roadElements.Select(re => new Road()
            {
                Elements = new List<RoadElement>() { re },
                GameObject = _gameObjectFactory.CreateNew(String.Format("road [{0}] {1}/ ", re.Id, re.Address), _tile.GameObject),
            }).ToArray();

            if (_tile.HeightMap.IsFlat)
                _tile.HeightMap.MaxElevation = rule.GetHeight();

            _terrainBuilder.Build(_tile.GameObject, new TerrainSettings()
            {
                Tile = _tile,
                AlphaMapSize = rule.GetAlphaMapSize(),
                CenterPosition = new Vector2(_tile.MapCenter.X, _tile.MapCenter.Y),
                CornerPosition = new Vector2(_tile.BottomLeft.X, _tile.BottomLeft.Y),
                PixelMapError = rule.GetPixelMapError(),
                ZIndex = rule.GetZIndex(),
                TextureParams = rule.GetTextureParams(),
                Areas = _areas,
                Elevations = _elevations,
                Roads = roads,
                RoadBuilder = _roadBuilder,
                RoadStyleProvider = _themeProvider.Get().GetStyleProvider<IRoadStyleProvider>()
            });
        }

        private void CreateSkipped(IGameObject parent, Model model)
        {
            // TODO this is useful only for debug, in release we should avoid creation of 
            // additional objects due to performance reason
            var skippedGameObject = _gameObjectFactory.CreateNew(String.Format("skip {0}", model));
            skippedGameObject.Parent = parent;
        }

        #endregion
    }
}
