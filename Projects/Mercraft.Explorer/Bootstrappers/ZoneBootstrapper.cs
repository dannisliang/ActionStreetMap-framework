using Mercraft.Core;
using Mercraft.Core.Scene;
using Mercraft.Core.Tiles;
using Mercraft.Infrastructure.Bootstrap;

namespace Mercraft.Explorer.Bootstrappers
{
    public class ZoneBootstrapper: BootstrapperPlugin
    {
        private const string SceneBuilderKey = "scene";
        private const string TileProviderKey = "provider";
        private const string PositionListenerKey = "loader";
        private const string SceneModelVisitorsKey = "visitors/visitor";

        public override bool Run()
        {
            Configurator.RegisterComponent<ISceneBuilder>(ConfigSection.GetSection(SceneBuilderKey));
            Configurator.RegisterComponent<TileProvider>(ConfigSection.GetSection(TileProviderKey));
            Configurator.RegisterComponent<IPositionListener>(ConfigSection.GetSection(PositionListenerKey));

            // register visitors
            foreach (var builderConfig in ConfigSection.GetSections(SceneModelVisitorsKey))
            {
                Configurator.RegisterNamedComponent<ISceneModelVisitor>(builderConfig);
            }

            return true;
        }
    }
}
