using System;

namespace Mercraft.Infrastructure.Bootstrap
{
    /// <summary>
    /// Provides the functionality to manage startup plugins
    /// </summary>
    public interface IBootstrapperService
    {
        /// <summary>
        /// Runs startup plugins
        /// </summary>
        /// <returns></returns>
        bool Run();

        /// <summary>
        /// Updates startup plugins
        /// </summary>
        /// <returns></returns>
        bool Update();

        /// <summary>
        /// Unloads plugins
        /// </summary>
        /// <returns></returns>
        bool Stop();
    }
}
