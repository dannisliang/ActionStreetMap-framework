using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;

namespace Mercraft.Infrastructure.Bootstrap
{
    /// <summary>
    /// Represents a bootstrapper plugin
    /// </summary>
    public abstract class BootstrapperPlugin: IBootstrapperPlugin
    {
        [Dependency]
        public IContainer Container { get; set; }

        [Dependency]
        public ConfigSettings Config { get; set; }

        public IConfigSection ConfigSection { get; set; }

        public string Name { get; private set; }

        protected BootstrapperPlugin(IConfigSection configSection)
        {
            ConfigSection = configSection;
            Name = configSection.GetString("@name");
        }

        #region IBootstrapperPlugin members

        public abstract bool Run();
        public abstract bool Update();
        public abstract bool Stop();

        #endregion
    }
}
