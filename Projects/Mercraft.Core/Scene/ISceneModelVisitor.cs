using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using UnityEngine;

namespace Mercraft.Core.Scene
{
    public interface ISceneModelVisitor
    {
        GameObject VisitCanvas(GeoCoordinate center, GameObject parent, Rule rule, Canvas canvas);
        GameObject VisitArea(GeoCoordinate center, GameObject parent, Rule rule, Area area);
        GameObject VisitWay(GeoCoordinate center, GameObject parent, Rule rule, Way way);
    }

    /// <summary>
    /// Helper class with trivial implementation
    /// </summary>
    public class SceneModelVisitor : ISceneModelVisitor
    {
        public virtual GameObject VisitCanvas(GeoCoordinate center, GameObject parent, Rule rule, Canvas canvas)
        {
            return null;
        }

        public virtual GameObject VisitArea(GeoCoordinate center, GameObject parent, Rule rule, Area area)
        {
            return null;
        }

        public virtual GameObject VisitWay(GeoCoordinate center, GameObject parent, Rule rule, Way way)
        {
            return null;
        }
    }

}
