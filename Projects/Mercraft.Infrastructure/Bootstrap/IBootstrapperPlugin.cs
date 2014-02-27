namespace Mercraft.Infrastructure.Bootstrap
{
    /// <summary>
    /// Represents a startup plugin
    /// </summary>
    public interface IBootstrapperPlugin
    {
        /// <summary>
        /// The name of plugin
        /// </summary>
        string Name { get; }

        #region Component root events

        bool Run();
        bool Update();
        bool Stop();

        #endregion
    }
}
