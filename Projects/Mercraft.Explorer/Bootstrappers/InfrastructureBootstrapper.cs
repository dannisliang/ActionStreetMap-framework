using Mercraft.Core.Unity;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Diagnostic;

namespace Mercraft.Explorer.Bootstrappers
{
    public class InfrastructureBootstrapper: BootstrapperPlugin
    {
        private const string LogKey = "log";
        private const string GameObjectFactoryKey = "goFactory";

        public override bool Run()
        {
            Configurator.RegisterComponent<ITrace>(ConfigSection.GetSection(LogKey));
            Configurator.RegisterComponent<IGameObjectFactory>(ConfigSection.GetSection(GameObjectFactoryKey));
            return true;
        }
    }
}
