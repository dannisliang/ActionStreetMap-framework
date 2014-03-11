using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Tiles;
using UnityEngine;

namespace Mercraft.Core.Zones
{
    public class Zone
    {
        private readonly Tile _tile;
        private readonly Stylesheet _stylesheet;
        private readonly ITerrainBuilder _terrainBuilder;
        private readonly IEnumerable<ISceneModelVisitor> _sceneModelVisitors;

        private GameObject _floor;

        public Zone(Tile tile, 
            Stylesheet stylesheet,
            ITerrainBuilder terrainBuilder,
            IEnumerable<ISceneModelVisitor> sceneModelVisitors)
        {
            _tile = tile;
            _stylesheet = stylesheet;
            _terrainBuilder = terrainBuilder;
            _sceneModelVisitors = sceneModelVisitors;
        }

        public void Build()
        {
            _floor = _terrainBuilder.Build(_tile);
            
            foreach (var area in _tile.Scene.Areas)
            {
                var rule = _stylesheet.GetRule(area);
                if (rule != null)
                {
                    foreach (var sceneModelVisitor in _sceneModelVisitors)
                    {

                        // TODO probably, we need to return built game object 
                        // to be able to perform cleanup on our side
                        sceneModelVisitor.VisitArea(_tile.RelativeNullPoint, _floor, rule, area);

                    }
                }
            }
        }
    }
}
