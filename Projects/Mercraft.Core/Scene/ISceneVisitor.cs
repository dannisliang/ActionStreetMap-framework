using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;

namespace Mercraft.Core.Scene
{
    /// <summary>
    ///     Represents behavior which builds game objects from given models and rules
    /// </summary>
    public interface ISceneVisitor
    {
        bool VisitCanvas(GeoCoordinate center, IGameObject parent, Rule rule, Canvas canvas);
        bool VisitArea(GeoCoordinate center, IGameObject parent, Rule rule, Area area);
        bool VisitWay(GeoCoordinate center, IGameObject parent, Rule rule, Way way);
    }
}