using Mercraft.Core.Unity;
using Mercraft.Models.Terrain;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    public class TestTerrainBuilder: TerrainBuilder
    {
        protected override IGameObject CreateTerrainGameObject(IGameObject parent, TerrainSettings settings, float[,] htmap)
        {
            return new TestGameObject();
        }
    }
}
