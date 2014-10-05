using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.Scene
{
    public interface ISceneBuilder
    {
        void Build(Tile tile, BoundingBox bbox);
    }
}
