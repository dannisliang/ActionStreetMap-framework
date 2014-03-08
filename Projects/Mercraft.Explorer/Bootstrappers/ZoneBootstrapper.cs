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
        private const string TerrainBuilderKey = "terrain";
        private const string PositionListenerKey = "loader";
        private const string SceneModelVisitorsKey = "builders/builder";

        public override bool Run()
        {
            Configurator.RegisterComponent<ISceneBuilder>(ConfigSection.GetSection(SceneBuilderKey));
            Configurator.RegisterComponent<TileProvider>(ConfigSection.GetSection(TileProviderKey));
            Configurator.RegisterComponent<ITerrainBuilder>(ConfigSection.GetSection(TerrainBuilderKey));
            Configurator.RegisterComponent<IPositionListener>(ConfigSection.GetSection(PositionListenerKey));

            // register builders
            foreach (var builderConfig in ConfigSection.GetSections(SceneModelVisitorsKey))
            {
                Configurator.RegisterNamedComponent<ISceneModelVisitor>(builderConfig);
            }

            return true;
        }

        public override bool Update()
        {
            return true;
        }

        public override bool Stop()
        {
            return true;
        }
    }
}
