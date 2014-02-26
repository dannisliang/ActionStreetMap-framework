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

        bool Load();
        bool Update();
        bool Unload();

        #endregion
    }
}
