using System;
using Mercraft.Core.Scene.World.Buildings;
using Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs;
using Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs.Builders;
using Mercraft.Models.Buildings;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Models.Buildings
{
    [TestFixture]
    public class DomeRoofBuilder
    {
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CanThrowExceptionIfNoHeight()
        {
            // ARRANGE
            var roofBuilder = new TestDomeRoofBuilder(new TestGameObjectFactory());
            var building = new Building()
            {
                RoofHeight = 0
            };
            var style = new BuildingStyle();

            // ACT
            roofBuilder.Build(building, style);
        }
    }
}
