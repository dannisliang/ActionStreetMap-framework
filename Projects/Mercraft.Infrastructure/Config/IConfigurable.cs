namespace Mercraft.Infrastructure.Config
{
    public interface IConfigurable
    {
        void Configure(IConfigSection config);
    }
}
