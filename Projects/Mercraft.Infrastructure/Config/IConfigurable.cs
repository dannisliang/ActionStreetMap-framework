namespace ActionStreetMap.Infrastructure.Config
{
    /// <summary>
    ///     Configurable through DI container class
    /// </summary>
    public interface IConfigurable
    {
        /// <summary>
        ///     Configures object using configuration section.
        /// </summary>
        /// <param name="configSection">Configuration section.</param>
        void Configure(IConfigSection configSection);
    }
}