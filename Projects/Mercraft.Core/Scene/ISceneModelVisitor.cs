using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.Scene
{
    public interface ISceneModelVisitor
    {
        void VisitBuilding(Building building);
        void VisitRoad(); // TODO
    }
}
