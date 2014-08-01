using System.Collections.Generic;
using System.Linq;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;

namespace Mercraft.Infrastructure.Bootstrap
{
    /// <summary>
    /// Provides default functionality to execute startup plugins
    /// </summary>
    public class BootstrapperService: IBootstrapperService
    {
        private readonly IEnumerable<IBootstrapperPlugin> _plugins;

        [Dependency]
        public IContainer Container { get; set; }

        [Dependency]
        public BootstrapperService() { }

        public BootstrapperService(Container container, IEnumerable<IBootstrapperPlugin> plugins)
        {
            Container = container;
            _plugins = plugins;
        }

        #region IBootstrapperService members

        /// <summary>
        /// Run all registred bootstrappers
        /// </summary>
        /// <returns></returns>
        public bool Run()
        {
            var plugins = _plugins ?? Container.ResolveAll<IBootstrapperPlugin>();
            return plugins
                .ToList().Aggregate(true, (current, task) => current & task.Run());
        }

        /// <summary>
        /// Updates all registred bootstrappers
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            var plugins = _plugins ?? Container.ResolveAll<IBootstrapperPlugin>();
            return plugins
                .ToList().Aggregate(true, (current, task) => current & task.Update());
        }

        /// <summary>
        /// Updates all registred bootstrappers
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            var plugins = _plugins ?? Container.ResolveAll<IBootstrapperPlugin>();
            return plugins
                .ToList().Aggregate(true, (current, task) => current & task.Stop());
        }

        #endregion
    }
}
