using Mercraft.Core.Unity;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Buildings;
using Mercraft.Models.Utils;
using Color32 = UnityEngine.Color32;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs.Builders
{
    public class TestBuildingBuilder: BuildingBuilder
    {
        [Dependency]
        public TestBuildingBuilder(IResourceProvider resourceProvider) : base(resourceProvider)
        {
        }

        protected override void AttachChildGameObject(IGameObject parent, string name, MeshData meshData, Color32 color)
        {
            // Do nothing
        }
    }
}
