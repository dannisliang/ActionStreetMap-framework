using Mercraft.Core;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Scene.Builders;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs.ModelBuilders
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
