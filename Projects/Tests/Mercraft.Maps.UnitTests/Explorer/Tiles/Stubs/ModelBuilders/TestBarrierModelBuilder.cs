using System.Collections.Generic;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Scene.Builders;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs.ModelBuilders
{
    class TestBarrierModelBuilder: BarrierModelBuilder
    {
        protected override void BuildObject(IGameObject gameObjectWrapper, Rule rule, 
            List<Vector3> p, List<int> t, List<Vector2> u)
        {
            // Do nothing
        }
    }
}
