namespace Mercraft.Infrastructure.Config
{
    /// <summary>
    /// Configurable through DI container class
    /// </summary>
    public interface IConfigurable
    {
        void Configure(IConfigSection configSection);
    }
}
