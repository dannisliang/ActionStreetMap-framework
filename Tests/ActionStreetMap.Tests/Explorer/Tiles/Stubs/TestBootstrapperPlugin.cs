using ActionStreetMap.Core.Scene;
using ActionStreetMap.Core.Unity;
using ActionStreetMap.Explorer.Scene;
using ActionStreetMap.Infrastructure.Bootstrap;
using ActionStreetMap.Infrastructure.Dependencies;
using ActionStreetMap.Infrastructure.Diagnostic;
using ActionStreetMap.Tests.Explorer.Tiles.Stubs.Behaviours;
using ActionStreetMap.Tests.Explorer.Tiles.Stubs.Builders;
using ActionStreetMap.Tests.Explorer.Tiles.Stubs.ModelBuilders;
using ActionStreetMap.Models.Buildings;
using ActionStreetMap.Models.Buildings.Roofs;
using ActionStreetMap.Models.Roads;
using ActionStreetMap.Models.Terrain;

namespace ActionStreetMap.Tests.Explorer.Tiles.Stubs
{
    /// <summary>
    ///     This plugin overrides registration of non-testable classes
    /// </summary>
    public class TestBootstrapperPlugin: BootstrapperPlugin
    {
        private readonly TestModelBehaviour _solidModelBehaviour = new TestModelBehaviour("solid");
        private readonly TestModelBehaviour _waterModelBehaviour = new TestModelBehaviour("water");

        public override string Name
        {
            get { return "test"; }
        }

        public override bool Run()
        {
            Container.Register(Component.For<ITrace>().Use<ConsoleTrace>());

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

            Container.Register(Component.For<IRoofBuilder>().Use<TestDomeRoofBuilder>().Named("dome"));

            Container.RegisterInstance<IModelBehaviour>(_solidModelBehaviour, "solid");
            Container.RegisterInstance<IModelBehaviour>(_waterModelBehaviour, "water");

            return true;
        }
    }
}
