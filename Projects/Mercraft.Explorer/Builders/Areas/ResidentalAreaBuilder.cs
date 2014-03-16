
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Explorer.Builders.Areas.Generators;
using Mercraft.Explorer.Helpers;
using UnityEngine;

namespace Mercraft.Explorer.Builders.Areas
{
    public class ResidentialAreaBuilder : ModelBuilder
    {
        private readonly SovietBuildingGenerator _generator = new SovietBuildingGenerator();

        public override void BuildArea(GeoCoordinate center, GameObject gameObject, Rule rule, Area area)
        {
            var levels = rule.GetLevels(area);
            var settings = new BuildingSettings()
            {
                Levels = 5,
                Entrances = 1,
                CeilingHeight = 3,
                Attic = false,
                SocleHeight = 1,
                EntranceMeshIndex = 0,
                EntranceWallMeshIndex = 0,
                EntranceWallLastMeshIndex = 0,
            };

            var vertices = PolygonHelper.GetVerticies2D(center, area.Points.ToList());

            _generator.Generate(gameObject, PolygonHelper.SortVertices(vertices), settings);
        }
     
    }
}
