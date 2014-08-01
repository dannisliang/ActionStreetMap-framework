using Mercraft.Core.Unity;
using Mercraft.Explorer.Infrastructure;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;

namespace Mercraft.Explorer.Bootstrappers
{
    public class InfrastructureBootstrapper: BootstrapperPlugin
    {
        public override string Name { get { return "infrastructure"; } }

        public override bool Run()
        {
            Container.Register(Component.For<ITrace>().Use<DefaultTrace>().Singleton());
            Container.Register(Component.For<IGameObjectFactory>().Use<GameObjectFactory>().Singleton());

            return true;
        }
    }
}
