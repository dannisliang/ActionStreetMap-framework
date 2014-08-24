using System;
using Mercraft.Core.Scene;
using Mercraft.Core.Unity;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;
using Mercraft.Models.Buildings;
using Mercraft.Models.Roads;
using Mercraft.Models.Terrain;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    /// <summary>
    ///     This plugin overrides registration of non-testable classes
    /// </summary>
    public class TestBootstrapperPlugin: BootstrapperPlugin
    {
        public override string Name
        {
            get { return "test"; }
        }

        public override bool Run()
        {
            Container.Register(Component.For<ITrace>().Use<DefaultTrace>());

            Container.Register(Component.For<IGameObjectFactory>().Use<TestGameObjectFactory>());

            Container.Register(Component.For<IModelBuilder>().Use<TestFlatModelBuilder>().Named("flat"));

            Container.Register(Component.For<ITerrainBuilder>().Use<TestTerrainBuilder>());
            Container.Register(Component.For<IBuildingBuilder>().Use<TestBuildingBuilder>());
            Container.Register(Component.For<IRoadBuilder>().Use<TestRoadBuilder>());

            Container.RegisterInstance<IModelBehaviour>(new TestModelBehaviour("solid"), "solid");
            Container.RegisterInstance<IModelBehaviour>(new TestModelBehaviour("water"), "water");

            return true;
        }
    }
}
