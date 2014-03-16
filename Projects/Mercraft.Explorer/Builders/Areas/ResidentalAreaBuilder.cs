
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
        private readonly SovietBuildingResources _resources;
        private readonly SovietBuildingGenerator _generator;

        public ResidentialAreaBuilder()
        {
            _resources = new SovietBuildingResources();
            _generator = new SovietBuildingGenerator(_resources);
        }

        public override void BuildArea(GeoCoordinate center, GameObject gameObject, Rule rule, Area area)
        {
            var levels = rule.GetLevels(area);
            var random = new System.Random();
            var settings = new SovietBuildingSettings()
            {
                Levels = 5,
                Entrances = 0, // will calculate later based on side length
                CeilingHeight = 3,
                Attic = Random.value > 0.5f,
                SocleHeight = 1,
                EntranceMeshIndex = random.Next(_resources.Entrance25.Count),
                EntranceWallMeshIndex = random.Next(_resources.EntranceWall25.Count),
                EntranceWallLastMeshIndex = random.Next(_resources.EntranceWallLast25.Count),
            };

            var vertices = PolygonHelper.GetVerticies2D(center, area.Points.ToList());

            _generator.Generate(gameObject, vertices, settings);
        }
     
    }
}
