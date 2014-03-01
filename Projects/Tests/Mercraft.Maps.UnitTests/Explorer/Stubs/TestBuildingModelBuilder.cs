using Mercraft.Core;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Explorer.Stubs
{
    public class TestBuildingModelVisitor: ISceneModelVisitor
    {
        public void VisitBuilding(GeoCoordinate center, GameObject parent, Building building)
        {
            
        }

        public void VisitRoad(GeoCoordinate center, GameObject parent, Road road)
        {
            
        }
    }
}
