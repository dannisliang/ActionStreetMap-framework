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
            // NOTE this is rough cchecking and should be improved
            Assert.AreEqual(2, simplified.Count);
        }
    }
}
