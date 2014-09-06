using System;
using Mercraft.Core.Scene;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Scene;
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
        private TestModelBehaviour _solidModelBehaviour = new TestModelBehaviour("solid");
        private TestModelBehaviour _waterModelBehaviour = new TestModelBehaviour("water");

        public override string Name
        {
            get { return "test"; }
        }

        public override bool Run()
        {
            Container.Register(Component.For<ITrace>().Use<DefaultTrace>());

            Container.Register(Component.For<IGameObjectFactory>().Use<TestGameObjectFactory>());

            Container.Register(Component.For<IModelBuilder>().Use<TestFlatModelBuilder>().Named("flat"));
            Container.Register(Component.For<IModelBuilder>().Use<TestWaterModelBuilder>().Named("water"));

            Container.Register(Component.For<ITerrainBuilder>().Use<TestTerrainBuilder>());
            Container.Register(Component.For<IBuildingBuilder>().Use<TestBuildingBuilder>());
            Container.Register(Component.For<IRoadBuilder>().Use<TestRoadBuilder>());

            Container.RegisterInstance<IModelBehaviour>(_solidModelBehaviour, "solid");
            Container.RegisterInstance<IModelBehaviour>(_waterModelBehaviour, "water");

            return true;
        }
    }
}
