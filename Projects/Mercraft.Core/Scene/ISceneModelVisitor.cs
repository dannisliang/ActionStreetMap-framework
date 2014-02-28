using Mercraft.Core.Scene.Models;
using UnityEngine;

namespace Mercraft.Core.Scene
{
    public interface ISceneModelVisitor
    {
        void VisitBuilding(GeoCoordinate center, GameObject parent, Building building);
        void VisitRoad(GeoCoordinate center, GameObject parent, Road road);
    }
}
