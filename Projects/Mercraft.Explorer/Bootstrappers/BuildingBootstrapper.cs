
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Buildings.Config;

namespace Mercraft.Explorer.Bootstrappers
{
    /// <summary>
    /// Registers building specific logic
    /// </summary>
    public class BuildingBootstrapper : BootstrapperPlugin
    {
        private const string BuildingStyleKey = "styles";
        private const string BuildingTextureKey = "textures";

        public override bool Run()
        {
            Container.Register(Component.For<BuildingStyleProvider>()
                .Use<BuildingStyleProvider>(new object[] { ConfigSection.GetSection(BuildingStyleKey) }));

            Container.Register(Component.For<TexturePackProvider>()
                .Use<TexturePackProvider>(new object[] { ConfigSection.GetSection(BuildingTextureKey) }));

            return true;
        }
    }
}
