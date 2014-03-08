using Mercraft.Explorer.Render;
using Mercraft.Infrastructure.Bootstrap;

namespace Mercraft.Explorer.Bootstrappers
{
    public class SceneBootstrapper: BootstrapperPlugin
    {

        private const string MeshBuildersKey = "meshes/builders/builder";
        private const string MeshRendersKey = "meshes/renders/render";

        public override bool Run()
        {
            //NOTE: No interface yet
            // TODO extract interface if possible
            // register mesh builders
            foreach (var builderConfig in ConfigSection.GetSections(MeshBuildersKey))
            {
                Configurator.RegisterComponent(builderConfig);
            }

            // register mesh renders
            foreach (var renderConfig in ConfigSection.GetSections(MeshRendersKey))
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
