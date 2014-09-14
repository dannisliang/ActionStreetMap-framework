using Mercraft.Core.Unity;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Terrain;
using Mercraft.Models.Utils;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    public class TestTerrainBuilder: TerrainBuilder
    {
        [Dependency]
        public TestTerrainBuilder(IResourceProvider resourceProvider) : base(resourceProvider)
        {
        }

        protected override IGameObject CreateTerrainGameObject(IGameObject parent, TerrainSettings settings, float[,] htmap)
        {
            return new TestGameObject();
        }
    }
}
