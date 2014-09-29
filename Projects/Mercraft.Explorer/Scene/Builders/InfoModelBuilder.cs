using System;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Core.World;
using Mercraft.Core.World.Infos;
using Mercraft.Explorer.Helpers;
using Mercraft.Explorer.Themes;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Infos;
using Mercraft.Models.Utils;

namespace Mercraft.Explorer.Scene.Builders
{
    public class InfoModelBuilder: ModelBuilder
    {
        private readonly IThemeProvider _themeProvider;
        private readonly IResourceProvider _resourceProvider;

        public override string Name
        {
            get { return "info"; }
        }

        [Dependency]
        public InfoModelBuilder(WorldManager worldManager, IGameObjectFactory gameObjectFactory,
            IThemeProvider themeProvider,
            IResourceProvider resourceProvider) :
                base(worldManager, gameObjectFactory)
        {
            _themeProvider = themeProvider;
            _resourceProvider = resourceProvider;
        }

        public override IGameObject BuildNode(Tile tile, Rule rule, Node node)
        {
            var key = rule.GetKey();
            var info = new Info()
            {
                Key = key,
                Coordinate = node.Point,
            };
            var style = _themeProvider.Get().GetInfoStyle(info);

            var gameObjectWrapper = GameObjectFactory.CreateNew(String.Format("info {0}", node));

            BuildObject(gameObjectWrapper, info, style);

            return gameObjectWrapper;
        }

        protected virtual void BuildObject(IGameObject gameObjectWrapper, Info info, InfoStyle style)
        {
            // TODO 
        }
    }
}
