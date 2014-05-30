using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;

namespace Mercraft.Core.Scene
{
    /// <summary>
    /// Represents behavior which builds game objects from given models and rules
    /// </summary>
    public interface IGameObjectBuilder
    {
        IGameObject FromCanvas(GeoCoordinate center, IGameObject parent, Rule rule, Canvas canvas);
        IGameObject FromArea(GeoCoordinate center, IGameObject parent, Rule rule, Area area);
        IGameObject FromWay(GeoCoordinate center, IGameObject parent, Rule rule, Way way);
    }
}
