using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;

namespace Mercraft.Core.Scene
{
    /// <summary>
    ///     Represents behavior which builds game objects from given models and rules
    /// </summary>
    public interface ISceneVisitor
    {
        void Prepare(IScene scene, Stylesheet stylesheet);
        void Finalize(IScene scene);

        SceneVisitResult VisitCanvas(Tile tile, Rule rule, Canvas canvas, bool visitedBefore);
        SceneVisitResult VisitArea(Tile tile, Rule rule, Area area, bool visitedBefore);
        SceneVisitResult VisitWay(Tile tile, Rule rule, Way way, bool visitedBefore);
    }
}