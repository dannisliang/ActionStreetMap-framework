﻿using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Unity;
using Mercraft.Core.World;
using Mercraft.Explorer.Scene.Builders;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs.ModelBuilders
{
    class TestWaterModelBuilder : WaterModelBuilder
    {
        [Dependency]
        public TestWaterModelBuilder(WorldManager worldManager, IGameObjectFactory gameObjectFactory,
            IResourceProvider resourceProvider) : base(worldManager, gameObjectFactory, resourceProvider)
        {
        }

        protected override void BuildObject(IGameObject gameObjectWrapper, Rule rule, Vector3[] points, int[] triangles)
        {
            // do nothing
        }
    }
}