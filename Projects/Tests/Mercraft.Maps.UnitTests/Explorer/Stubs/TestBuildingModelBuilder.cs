using Mercraft.Core;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Explorer.Stubs
{
    public class TestBuildingModelVisitor : ISceneModelVisitor
    {

        public void VisitArea(GeoCoordinate center, GameObject parent, Rule rule, Area area)
        {
            
        }

        public void VisitWay(GeoCoordinate center, GameObject parent, Rule rule, Way way)
        {
            
        }
    }
}
