using Mercraft.Core;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Scene.World.Infos;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Scene.Builders;
using Mercraft.Models.Infos;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs.ModelBuilders
{
    public class TestInfoModelBuilder : InfoModelBuilder
    {
        protected override void BuildObject(Tile tile, IGameObject gameObjectWrapper, Info info, 
            InfoStyle style, MapPoint mapPoint, float zIndex)
        {
            
        }
    }
}
