using Mercraft.Core;
using Mercraft.Models.Utils;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Algorithms
{
    [TestFixture]
    public class GeometryTests
    {
        [Test]
        public void CanSimplifyCurve()
        {
            // ARRANGE
            var points = new MapPoint[]
            {
                new MapPoint(0,0), 
                new MapPoint(0.5f,1), 
                new MapPoint(2,2), 
            };

            // ACT
            var simplified = Geometry.DouglasPeuckerReduction(points, 1);

            // ASSERT
            // NOTE this is rough checking and should be improved
            Assert.AreEqual(2, simplified.Count);
        }

        [Test]
        public void CanSimplifyStraitLine()
        {
            // ARRANGE
            var points = new MapPoint[]
            {
                new MapPoint(0,0), 
                new MapPoint(0, 10), 
                new MapPoint(0, 20), 
                new MapPoint(0, 40),
                new MapPoint(0, 80), 
            };

            // ACT
            var simplified = Geometry.DouglasPeuckerReduction(points, 1);

            // ASSERT
            // NOTE this is rough cchecking and should be improved
            Assert.AreEqual(2, simplified.Count);
            Assert.AreEqual(new MapPoint(0, 0), simplified[0]);
            Assert.AreEqual(new MapPoint(0, 80), simplified[1]);
        }
    }
}
