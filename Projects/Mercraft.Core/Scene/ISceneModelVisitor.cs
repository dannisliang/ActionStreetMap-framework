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
}
