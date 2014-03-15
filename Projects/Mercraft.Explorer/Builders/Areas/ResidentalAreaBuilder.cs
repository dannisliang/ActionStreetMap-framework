﻿
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Explorer.Builders.Areas.Generators;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Config;
using UnityEngine;

namespace Mercraft.Explorer.Builders.Areas
{
    public class ResidentialAreaBuilder: IModelBuilder<Area>, IConfigurable
    {
        private const string NameKey = "@name";

        public string Name { get; private set; }

        private readonly SovietBuildingGenerator _generator = new SovietBuildingGenerator();

        public void Build(GeoCoordinate center, GameObject gameObject, Rule rule, Area area)
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

            _generator.Generate(gameObject, vertices, settings);
        }

        public void Configure(IConfigSection configSection)
        {
            Name = configSection.GetString(NameKey);
        }
    }
}
