using Mercraft.Core.MapCss;
using Mercraft.Core.Scene;
using Mercraft.Explorer.Builders;
using Mercraft.Explorer.Interactions;
using Mercraft.Explorer.Themes;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Buildings;


namespace Mercraft.Explorer.Bootstrappers
{
    public class SceneBootstrapper: BootstrapperPlugin
    {
        private const string ThemesGlobalKey = "themes";
        private const string StylesheetProviderKey = "stylesheet";
        private const string BuildersKey = "builders/builder";
        private const string BehavioursKey = "behaviours/behaviour";
        private const string BuildingBuilderKey = "world/buildings/builder";
        private const string BuildingStyleKey = "world/buildings/style";

        public override bool Run()
        {
            // register theme provider
            Container.Register(Component
                .For<IThemeProvider>()
                .Use<ThemeProvider>()
                .Singleton()
                .SetConfig(Config.GetSection(ThemesGlobalKey)));

            // register stylesheet provider
            Configurator.RegisterComponent<IStylesheetProvider>(ConfigSection.GetSection(StylesheetProviderKey));

            // register mesh builders
            foreach (var builderConfig in ConfigSection.GetSections(BuildersKey))
                Configurator.RegisterNamedComponent<IModelBuilder>(builderConfig);

            // register behaviours
            foreach (var behaviourConfig in ConfigSection.GetSections(BehavioursKey))
                Configurator.RegisterNamedComponent<IModelBehaviour>(behaviourConfig);

            // building specific
            Configurator.RegisterComponent<IBuildingBuilder>(ConfigSection.GetSection(BuildingBuilderKey));
            Configurator.RegisterComponent<IBuildingStyleProvider>(ConfigSection.GetSection(BuildingStyleKey));

            return true;
        }
    }
}
