using Mercraft.Core.Unity;
using Mercraft.Explorer.Infrastructure;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;

namespace Mercraft.Explorer.Bootstrappers
{
    public class InfrastructureBootstrapper: BootstrapperPlugin
    {
        private const string LogTypeKey = "log/@type";

        public override bool Run()
        {
            var logType = ConfigSection.GetType(LogTypeKey);
            Container.Register(Component.For<ITrace>().Use(logType, new object[0]).Singleton());
            Container.Register(Component.For<IGameObjectFactory>().Use<GameObjectFactory>().Singleton());
            return true;
        }
    }
}
