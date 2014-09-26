using System.Collections.Generic;
using Mercraft.Core.World.Roads;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Roads;
using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs
{
    public class TestRoadBuilder: RoadBuilder
    {
        [Dependency]
        public TestRoadBuilder(IResourceProvider resourceProvider) : base(resourceProvider)
        {

        }

        protected override void CreateMesh(Road road, RoadStyle style, List<Vector3> points, List<int> triangles, List<Vector2> uv)
        {
        }
    }
}
