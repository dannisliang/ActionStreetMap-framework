using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.Scene
{
    public interface ITileActivator
    {
        void Activate(Tile tile);
        void Deactivate(Tile tile);
        void Destroy(Tile tile);
    }
}
