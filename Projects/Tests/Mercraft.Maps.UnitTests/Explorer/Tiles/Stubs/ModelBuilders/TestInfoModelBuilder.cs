using Mercraft.Core.Unity;
using Mercraft.Core.World;
using Mercraft.Core.World.Infos;
using Mercraft.Explorer.Scene.Builders;
using Mercraft.Explorer.Themes;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Infos;
using Mercraft.Models.Utils;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs.ModelBuilders
{
    public class TestInfoModelBuilder : InfoModelBuilder
    {
        [Dependency]
        public TestInfoModelBuilder(WorldManager worldManager, IGameObjectFactory gameObjectFactory,
            IThemeProvider themeProvider, IResourceProvider resourceProvider) :
                base(worldManager, gameObjectFactory, themeProvider, resourceProvider)
        {
        }

        protected override void BuildObject(IGameObject gameObjectWrapper, Info info, InfoStyle style)
        {
            // do nothing
        }
    }
}
