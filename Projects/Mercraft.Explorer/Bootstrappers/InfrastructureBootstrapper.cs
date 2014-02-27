
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;

namespace Mercraft.Explorer.Bootstrappers
{
    public class InfrastructureBootstrapper: BootstrapperPlugin
    {
        public InfrastructureBootstrapper(IConfigSection configSection) : base(configSection)
        {
        }

        public override bool Run()
        {
            var logType = ConfigSection.GetType("log/@type");
            Container.Register(Component.For<ITrace>().Use(logType, new object[0]).Singleton());
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
