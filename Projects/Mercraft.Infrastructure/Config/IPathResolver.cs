namespace Mercraft.Infrastructure.Config
{
    public interface IPathResolver
    {
        string Resolve(string path);
    }
}