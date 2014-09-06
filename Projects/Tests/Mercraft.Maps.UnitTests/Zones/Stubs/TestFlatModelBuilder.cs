using Mercraft.Core;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Scene.Builders;
using Mercraft.Infrastructure.Dependencies;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    public class TestFlatModelBuilder : FlatModelBuilder
    {
        public override string Name
        {
            get { return "flat"; }
        }

        [Dependency]
        public TestFlatModelBuilder(IGameObjectFactory gameObjectFactory)
            : base(gameObjectFactory)
        {
        }

        protected override void ProcessGameObject(IGameObject gameObjectWrapper, Rule rule, 
            MapPoint[] verticies, float floor)
        {
        }
    }
}