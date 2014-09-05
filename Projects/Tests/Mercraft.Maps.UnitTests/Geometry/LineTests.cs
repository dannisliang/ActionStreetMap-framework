using System;
using Mercraft.Core;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Geometry
{
    [TestFixture]
    public class LineTests
    {
        [Test]
        public void CanGetIntermediatePoints()
        {
            // ARRANGE
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
           /* var result = LineUtils.GetIntermediatePoints(points, 1f);

            //ARRANGE
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.IsFalse(result.Length == points.Length);
            Assert.AreEqual(21, result.Length);*/
        }

        [Test]
        public void CanGetNextIntermediatePoint()
        {
            // ARRANGE & ACT
            /*var result = LineUtils.GetNextIntermediatePoint(new MapPoint(0, 0, 1), new MapPoint(2, 2, 2), 1);

            // ASSERT
            Assert.IsTrue(Math.Abs(0.7f -  result.X) < 0.01);
            Assert.IsTrue(Math.Abs(0.7f - result.Y) < 0.01);
            Assert.IsTrue(Math.Abs(1 - result.Elevation) < 0.01);*/
        }
    }
}
