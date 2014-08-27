using Mercraft.Core.Unity;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Roads;
using Mercraft.Models.Terrain;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    class TestTerrainBuilder: ITerrainBuilder
    {
        private readonly IRoadBuilder _roadBuilder;

        [Dependency]
        public TestTerrainBuilder(IRoadBuilder roadBuilder)
        {
            _roadBuilder = roadBuilder;
        }

        public IGameObject Build(IGameObject parent, TerrainSettings settings)
        {
            // process roads
            foreach (var road in settings.Roads)
            {
                var style = settings.RoadStyleProvider.Get(road);
                _roadBuilder.Build(road, style);
            }

            return null;
        }
    }
}
