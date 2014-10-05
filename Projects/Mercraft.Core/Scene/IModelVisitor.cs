using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.Scene
{
    public interface IModelVisitor
    {
        void VisitTile(Tile tile);

        void VisitArea(Area area);
        void VisitWay(Way way);
        void VisitNode(Node node);

        void VisitCanvas(Canvas canvas);
    }
}
