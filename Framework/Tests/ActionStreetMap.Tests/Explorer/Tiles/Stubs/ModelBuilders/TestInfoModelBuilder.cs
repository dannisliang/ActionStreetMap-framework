using ActionStreetMap.Core;
using ActionStreetMap.Core.Scene.Models;
using ActionStreetMap.Core.Scene.World.Infos;
using ActionStreetMap.Core.Unity;
using ActionStreetMap.Explorer.Scene.Builders;
using ActionStreetMap.Models.Infos;

namespace ActionStreetMap.Tests.Explorer.Tiles.Stubs.ModelBuilders
{
    public class TestInfoModelBuilder : InfoModelBuilder
    {
        protected override void BuildObject(Tile tile, IGameObject gameObjectWrapper, Info info, 
            InfoStyle style, MapPoint mapPoint, float zIndex)
        {
            
        }
    }
}
