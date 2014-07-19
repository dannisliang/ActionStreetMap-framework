using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Core.World.Roads;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Areas;
using Mercraft.Models.Terrain;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    /// <summary>
    ///     Emulates real SceneVisitor without unity specific classes which throw SecurityException
    /// </summary>
    public class TestSceneVisitor : ISceneVisitor
    {
        private readonly IGameObjectFactory _goFactory;
        private readonly IEnumerable<IModelBuilder> _builders;
        private readonly IEnumerable<IModelBehaviour> _behaviours;

        private List<RoadElement> _roadElements = new List<RoadElement>();

        [Dependency]
        public TestSceneVisitor(IGameObjectFactory goFactory,
            IEnumerable<IModelBuilder> builders,
            IEnumerable<IModelBehaviour> behaviours)
        {
            _goFactory = goFactory;
            _builders = builders.ToList();
            _behaviours = behaviours.ToList();
        }

        #region ISceneVisitor implementation

        public bool VisitCanvas(GeoCoordinate center, IGameObject parent, Rule rule, Canvas canvas)
        {
            var tile = canvas.Tile;

            var roads = RoadElementComposer.Compose(_roadElements).Select(reList => new Road()
            {
                Elements = reList,
                GameObject = _goFactory.CreateNew(reList.Aggregate(new StringBuilder("road "),
                (sb, re) => sb.AppendFormat("{0}/ ", re.Address)).ToString())
            }).ToArray();

            var terrainBuilder = new TerrainBuilder(new TerrainSettings()
            {
                AlphaMapSize = rule.GetAlphaMapSize(),
                CenterPosition = new Vector2(tile.TileMapCenter.X, tile.TileMapCenter.Y),
                TerrainSize = tile.Size,
                SplatPrototypes = rule.GetSplatPrototypes(),
                //Areas = _areas,
                Roads = roads
            });

            // NOTE not ideal solution to make class ready for next request
            //_areas = new List<AreaSettings>();
            _roadElements = new List<RoadElement>();

            //terrainBuilder.Build(parent);
            return true;
        }

        public bool VisitArea(GeoCoordinate center, IGameObject parent, Rule rule, Area area)
        {
            if (rule.IsSkipped())
                return true;

            if (rule.IsTerrain())
            {
                return false;
            }

            var builder = rule.GetModelBuilder(_builders);
            var gameObjectWrapper = builder.BuildArea(center, rule, area);
            ApplyBehaviour(gameObjectWrapper, rule, area);

            return true;
        }

        public bool VisitWay(GeoCoordinate center, IGameObject parent, Rule rule, Way way)
        {
            if (rule.IsSkipped())
                return true;

            if (rule.IsRoad())
            {
                var roadGameObject = _goFactory.CreateNew(way.ToString());
                _roadElements.Add(new RoadElement()
                {
                    Width = (int)Math.Round(rule.GetWidth()),
                    Points = way.Points.Select(p => GeoProjection.ToMapCoordinate(center, p)).ToArray()
                });
                // this game object should be initialized inside of TerrainBuilder's logic
                return true;
            }

            var builder = rule.GetModelBuilder(_builders);
            var gameObjectWrapper = builder.BuildWay(center, rule, way);
            ApplyBehaviour(gameObjectWrapper, rule, way);

            return true;
        }

        #endregion

        private void ApplyBehaviour(IGameObject target, Rule rule, Model model)
        {
            var behaviour = rule.GetModelBehaviour(_behaviours);
            if (behaviour != null)
                behaviour.Apply(target, model);
        }
    }
}