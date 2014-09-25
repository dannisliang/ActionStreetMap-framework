using System;
using Mercraft.Core;
using Mercraft.Core.Elevation;
using Mercraft.Models.Geometry.ThickLine;
using Moq;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Geometry
{
    [TestFixture]
    public class ThickLineTests
    {
        [Test]
        public void CanGetIntermediatePoints()
        {
            // ARRANGE
            var heightMapMock = new Mock<HeightMap>();
            heightMapMock.Setup(h => h.LookupHeight(It.IsAny<MapPoint>())).Returns(0);
            var points = new MapPoint[]
            {
                new MapPoint(0, 0, 1),
                new MapPoint(2, 2, 2),
                new MapPoint(3, 3, 3),
                new MapPoint(5, 5, 4),
                new MapPoint(7, 7, 5),
                new MapPoint(13, 13, 6)
            };

            // ACT
            var result = ThickLineUtils.GetIntermediatePoints(heightMapMock.Object, points, 1f, 1f);

            //ARRANGE
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.IsFalse(result.Length == points.Length);
            Assert.AreEqual(16, result.Length); // TODO check corectness
        }

        [Test]
        public void CanGetNextIntermediatePoint()
        {
            // ARRANGE & ACT
            var heightMapMock = new Mock<HeightMap>();
            heightMapMock.Setup(h => h.LookupHeight(It.IsAny<MapPoint>())).Returns(0);
            var result = ThickLineUtils.GetNextIntermediatePoint(heightMapMock.Object, 
                new MapPoint(0, 0, 1), new MapPoint(2, 2, 2), 1);

            // ASSERT
            Assert.IsTrue(Math.Abs(0.7f -  result.X) < 0.01);
            Assert.IsTrue(Math.Abs(0.7f - result.Y) < 0.01);
        }
    }
}
