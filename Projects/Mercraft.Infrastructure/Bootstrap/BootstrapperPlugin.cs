using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;

namespace Mercraft.Infrastructure.Bootstrap
{
    /// <summary>
    ///     Represents a bootstrapper plugin
    /// </summary>
    public abstract class BootstrapperPlugin: IBootstrapperPlugin
    {
        [Dependency]
        public IContainer Container { get; set; }

        [Dependency]
        public IConfigSection GlobalConfigSection { get; set; }

        [Dependency]
        public IPathResolver PathResolver { get; set; }

        public abstract string Name { get; }

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
    }
}
