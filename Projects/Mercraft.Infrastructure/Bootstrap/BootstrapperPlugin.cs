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

        public string Name { get; private set; }

        protected BootstrapperPlugin(string name)
        {
            Name = name;
        }

        [Dependency]
        public ITrace Trace { get; set; }

        [Dependency]
        public TraceCategory Category { get; set; }

        #region IBootstrapperPlugin members

        public abstract bool Load();
        public abstract bool Update();
        public abstract bool Unload();

        #endregion
    }
}
