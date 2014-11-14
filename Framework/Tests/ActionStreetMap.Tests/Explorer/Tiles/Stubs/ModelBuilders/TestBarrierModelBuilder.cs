using System.Collections.Generic;
using ActionStreetMap.Core.MapCss.Domain;
using ActionStreetMap.Core.Unity;
using ActionStreetMap.Explorer.Scene.Builders;
using UnityEngine;

namespace ActionStreetMap.Tests.Explorer.Tiles.Stubs.ModelBuilders
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
