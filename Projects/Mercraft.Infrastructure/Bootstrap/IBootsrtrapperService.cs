namespace Mercraft.Infrastructure.Bootstrap
{
    /// <summary>
    ///     Provides the functionality to manage startup plugins
    /// </summary>
    public interface IBootstrapperService
    {
        /// <summary>
        ///     Runs startup plugins
        /// </summary>
        bool Run();

        /// <summary>
        ///     Updates startup plugins
        /// </summary>
        bool Update();

        /// <summary>
        ///     Unloads plugins
        /// </summary>
        bool Stop();
    }
}