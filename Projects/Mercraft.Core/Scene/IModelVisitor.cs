using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.Scene
{
    public interface IModelVisitor
    {
        void VisitArea(Area area);
        void VisitWay(Way way);
        void VisitCanvas(Canvas canvas);
    }
}
