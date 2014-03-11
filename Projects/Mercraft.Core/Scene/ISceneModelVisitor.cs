using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using UnityEngine;

namespace Mercraft.Core.Scene
{
    public interface ISceneModelVisitor
    {
        void VisitArea(GeoCoordinate center, GameObject parent, Rule rule,  Area area);
        void VisitWay(GeoCoordinate center, GameObject parent, Rule rule, Way way);
    }
}
