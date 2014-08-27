using Mercraft.Core.Unity;
using Mercraft.Infrastructure.Dependencies;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    class TestWaterModelBuilder : TestFlatModelBuilder
    {
        public override string Name
        {
            get { return "water"; }
        }

        [Dependency]
        public TestWaterModelBuilder(IGameObjectFactory gameObjectFactory) : base(gameObjectFactory)
        {
        }      
    }
}
