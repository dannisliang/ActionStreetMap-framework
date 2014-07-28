using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Utilities;

namespace Mercraft.Infrastructure.Bootstrap
{
    /// <summary>
    /// Represents a bootstrapper plugin
    /// </summary>
    public abstract class BootstrapperPlugin: IBootstrapperPlugin, IConfigurable
    {
        [Dependency]
        public IContainer Container { get; set; }

        [Dependency]
        public ConfigSettings Config { get; set; }

        [Dependency]
        public ComponentConfigurator Configurator { get; set; }

        public IConfigSection ConfigSection { get; set; }

        public string Name { get; private set; }

        #region IBootstrapperPlugin members

        public abstract bool Run();

        public virtual bool Update()
        {
            return false;
        }

        public virtual bool Stop()
        {
            return false;
        }

        #endregion

        public virtual void Configure(IConfigSection configSection)
        {
            ConfigSection = configSection;
            Name = configSection.GetString(ConfigKeys.Name);
        }
    }
}
