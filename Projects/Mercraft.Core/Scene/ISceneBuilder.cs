
namespace Mercraft.Core.Scene
{
    public interface ISceneBuilder
    {
        IScene Build(BoundingBox bbox);
    }
}
