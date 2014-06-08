using Mercraft.Core;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Core
{
    [TestFixture]
    public class BoundingBoxTests
    {
        [Test]
        public void CanAddBbox()
        {
            var bbox1 = new BoundingBox(new GeoCoordinate(0,0), new GeoCoordinate(2, 2));
            var bbox2 = new BoundingBox(new GeoCoordinate(1, 1), new GeoCoordinate(3, 3));

            var result = bbox1 + bbox2;

            Assert.AreEqual(bbox1.MinPoint, result.MinPoint);
            Assert.AreEqual(bbox2.MaxPoint, result.MaxPoint);
        }
    }
}