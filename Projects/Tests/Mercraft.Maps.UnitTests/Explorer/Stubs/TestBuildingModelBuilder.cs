using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Explorer.Meshes;
using Mercraft.Explorer.Render;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Explorer.Stubs
{
    public class TestBuildingModelVisitor : ISceneModelVisitor
    { 

        public void VisitBuilding(GeoCoordinate center, GameObject parent, Building building)
        {
            
        }

        public void VisitRoad(GeoCoordinate center, GameObject parent, Road road)
        {
            
        }
    }
}
