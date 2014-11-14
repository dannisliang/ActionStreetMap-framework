using ActionStreetMap.Core.Unity;
using ActionStreetMap.Infrastructure.Dependencies;
using ActionStreetMap.Models.Buildings;
using ActionStreetMap.Models.Utils;
using Color32 = UnityEngine.Color32;

namespace ActionStreetMap.Maps.UnitTests.Explorer.Tiles.Stubs.Builders
{
    public class TestBuildingBuilder: BuildingBuilder
    {
        [Dependency]
        public TestBuildingBuilder(IResourceProvider resourceProvider) : base(resourceProvider)
        {
        }

        protected override void AttachChildGameObject(IGameObject parent, string name, MeshData meshData)
        {
            // Do nothing
        }
    }
}
