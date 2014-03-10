using Mercraft.Core.Scene.Models;
using UnityEngine;

namespace Mercraft.Core.Scene
{
    public interface ISceneModelVisitor
    {
        void VisitArea(GeoCoordinate center, GameObject parent, Area area);
        void VisitWay(GeoCoordinate center, GameObject parent, Way way);
    }
}
