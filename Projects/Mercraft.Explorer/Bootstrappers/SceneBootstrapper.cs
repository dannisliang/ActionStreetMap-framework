using Mercraft.Core.MapCss;
using Mercraft.Explorer.Builders;
using Mercraft.Infrastructure.Bootstrap;

namespace Mercraft.Explorer.Bootstrappers
{
    public class SceneBootstrapper: BootstrapperPlugin
    {
        private const string StylesheetProviderKey = "stylesheet";
        private const string BuildersKey = "builders/builder";

        public override bool Run()
        {
            // register stylesheet provider
            Configurator.RegisterComponent<IStylesheetProvider>(ConfigSection.GetSection(StylesheetProviderKey));

            // register mesh builders
            foreach (var builderConfig in ConfigSection.GetSections(BuildersKey))
            {
                Configurator.RegisterNamedComponent<IModelBuilder>(builderConfig);
            }

            return true;
        }
    }
}
