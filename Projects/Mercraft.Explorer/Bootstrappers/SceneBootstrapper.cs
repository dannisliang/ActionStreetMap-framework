using Mercraft.Core.MapCss;
using Mercraft.Core.Scene;
using Mercraft.Explorer.Themes;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Buildings;
using Mercraft.Models.Roads;
using Mercraft.Models.Terrain;

namespace Mercraft.Explorer.Bootstrappers
{
    public class SceneBootstrapper: BootstrapperPlugin
    {
        private const string StylesheetProviderKey = "stylesheet";
        private const string BuildersKey = "builders/builder";
        private const string BehavioursKey = "behaviours/behaviour";

        private const string TerrainBuilderKey = "world/terrain";

        private const string BuildingBuilderKey = "world/buildings/builder";
        private const string BuildingStyleKey = "world/buildings/style";

        private const string RoadBuilderKey = "world/roads/builder";
        private const string RoadStyleKey = "world/roads/style";

        public override bool Run()
        {
            // register theme provider
            Container.Register(Component
                .For<IThemeProvider>()
                .Use<ThemeProvider>()
                .Singleton()
                .SetConfig(Config.GetRoot()));

            // register stylesheet provider
            Configurator.RegisterComponent<IStylesheetProvider>(ConfigSection.GetSection(StylesheetProviderKey));

            // register model builders
            foreach (var builderConfig in ConfigSection.GetSections(BuildersKey))
                Configurator.RegisterNamedComponent<IModelBuilder>(builderConfig);

            // register behaviours
            foreach (var behaviourConfig in ConfigSection.GetSections(BehavioursKey))
                Configurator.RegisterNamedComponent<IModelBehaviour>(behaviourConfig);

            // buildings
            Configurator.RegisterComponent<IBuildingBuilder>(ConfigSection.GetSection(BuildingBuilderKey));

            // terrain
            Configurator.RegisterComponent<ITerrainBuilder>(ConfigSection.GetSection(TerrainBuilderKey));
            
            // roads
            Configurator.RegisterComponent<IRoadBuilder>(ConfigSection.GetSection(RoadBuilderKey));

            return true;
        }
    }
}
