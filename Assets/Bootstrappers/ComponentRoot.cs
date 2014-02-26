
using System.Linq;
using Assets.Infrastructure;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;

namespace Assets.Bootstrappers
{
    /// <summary>
    /// Represents application component root
    /// </summary>
    class ComponentRoot
    {
        private readonly IContainer _container;

        public ComponentRoot()
        {
            _container = new Container();

            BootstrapPlugin<InfrastructureBootstrapper>("Bootstrappers.Infrastructure");
            BootstrapPlugin<SettingsBootstrapper>("Bootstrappers.Settings");
            BootstrapPlugin<OsmBootstrapper>("Bootstrappers.Osm");
            BootstrapPlugin<SceneBootstrapper>("Bootstrappers.Scene");
        }

        private void BootstrapPlugin<T>(string name)
        {
            _container.Register(Component.For<IBootstrapperPlugin>()
                .Use<T>()
                .Named(name)
                .Singleton());

            _container.Resolve<IBootstrapperPlugin>(name).Load();
        }

        public IContainer Container
        {
            get
            {
                return _container;
            }
        }
    }
}
