using Mercraft.Explorer.Render;
using Mercraft.Infrastructure.Bootstrap;

namespace Mercraft.Explorer.Bootstrappers
{
    public class SceneBootstrapper: BootstrapperPlugin
    {
        public override bool Run()
        {
            //NOTE: No interface yet
            // TODO extract interface if possible
            // register mesh builders
            foreach (var builderConfig in ConfigSection.GetSections("meshes/builders/builder"))
            {
                Configurator.RegisterComponent(builderConfig);
            }

            // register mesh renders
            foreach (var renderConfig in ConfigSection.GetSections("meshes/renders/render"))
            {
                Configurator.RegisterNamedComponent<IMeshRenderer>(renderConfig);
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
