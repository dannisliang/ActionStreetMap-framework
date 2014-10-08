using Mercraft.Core.Scene;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Scene;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;
using Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs.Behaviours;
using Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs.Builders;
using Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs.ModelBuilders;
using Mercraft.Models.Buildings;
using Mercraft.Models.Roads;
using Mercraft.Models.Terrain;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs
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

            Container.Register(Component.For<ITileActivator>().Use<TestTileActivator>());

            Container.Register(Component.For<IModelBuilder>().Use<TestWaterModelBuilder>().Named("water"));
            Container.Register(Component.For<IModelBuilder>().Use<TestDetailModelBuilder>().Named("detail"));
            Container.Register(Component.For<IModelBuilder>().Use<TestSphereModelBuilder>().Named("sphere"));
            Container.Register(Component.For<IModelBuilder>().Use<TestCylinderModelBuilder>().Named("cylinder"));
            Container.Register(Component.For<IModelBuilder>().Use<TestBarrierModelBuilder>().Named("barrier"));
            Container.Register(Component.For<IModelBuilder>().Use<TestInfoModelBuilder>().Named("info"));

            Container.Register(Component.For<ITerrainBuilder>().Use<TestTerrainBuilder>());
            Container.Register(Component.For<IBuildingBuilder>().Use<TestBuildingBuilder>());
            Container.Register(Component.For<IRoadBuilder>().Use<TestRoadBuilder>());

            Container.RegisterInstance<IModelBehaviour>(_solidModelBehaviour, "solid");
            Container.RegisterInstance<IModelBehaviour>(_waterModelBehaviour, "water");

            return true;
        }
    }
}
