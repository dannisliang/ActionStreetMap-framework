using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.Scene
{
    public interface IModelVisitor
    {
        void VisitArea(Area area);
        void VisitWay(Way way);
        void VisitNode(Node node);
        void VisitCanvas(Canvas canvas);
    }
}
