using Mercraft.Explorer.Infrastructure;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;

namespace Mercraft.Explorer.Bootstrappers
{
    public class InfrastructureBootstrapper: IBootstrapperPlugin
    {
        public string Name { get { return "Bootstrappers.Infrastructure"; } }

        [Dependency]
        private IContainer Container { get; set; }

        public bool Load()
        {
            Container.Register(Component.For<ITrace>().Use<UnityConsoleTrace>().Named("").Singleton());
            Container.Register(Component.For<TraceCategory>().Use<TraceCategory>("Default").Singleton());
            return true;
        }

        public bool Update()
        {
            return true;
        }

        public bool Unload()
        {
            return true;
        }
    }
}
