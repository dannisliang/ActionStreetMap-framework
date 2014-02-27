using Mercraft.Explorer.Bootstrappers;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;

namespace Mercraft.Explorer
{
    /// <summary>
    /// Represents application component root
    /// </summary>
    public class ComponentRoot
    {
        private IContainer _container;

        public ComponentRoot()
        {
            InitializeDefault();
        }

        public ComponentRoot(string configPath)
        {
            var configSettings = new ConfigSettings(configPath);
            InitializeFromConfig(configSettings);
        }

        public ComponentRoot(ConfigSettings configSettings)
        {
            InitializeFromConfig(configSettings);
        }

        /// <summary>
        /// Creates bootstrappers from config
        /// </summary>
        /// <param name="configSettings"></param>
        private void InitializeFromConfig(ConfigSettings configSettings)
        {
        }

        /// <summary>
        /// Creates default bootstrappers
        /// </summary>
        private void InitializeDefault()
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

        // NOTE shouldn't be exposed
        public IContainer Container
        {
            get
            {
                return _container;
            }
        }
    }
}
