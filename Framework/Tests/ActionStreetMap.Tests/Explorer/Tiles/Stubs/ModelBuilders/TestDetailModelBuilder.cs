using ActionStreetMap.Core;
using ActionStreetMap.Core.MapCss.Domain;
using ActionStreetMap.Core.Scene.Models;
using ActionStreetMap.Core.Unity;
using ActionStreetMap.Explorer.Scene.Builders;

namespace ActionStreetMap.Maps.UnitTests.Explorer.Tiles.Stubs.ModelBuilders
{
    class TestDetailModelBuilder: DetailModelBuilder
    {
        protected override IGameObject BuildObject(Tile tile, Rule rule, Node node, MapPoint mapPoint, float zIndex, string detail)
        {
            // do nothing
            return null;
        }
    }
}
