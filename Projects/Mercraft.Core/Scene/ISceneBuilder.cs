
namespace Mercraft.Core.Scene
{
    public interface ISceneBuilder
    {
        IScene Build(GeoCoordinate center, BoundingBox bbox);
    }
}
