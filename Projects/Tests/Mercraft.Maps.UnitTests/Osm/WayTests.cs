using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Scene;
using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Visitors;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Osm
{
    [TestFixture]
    public class WayTests
    {
        [Test]
        public void CanFindWays()
        {
            // ARRANGE
            var dataSource = new PbfIndexListElementSource(TestHelper.TestBigPbfIndexListPath,
                new TestPathResolver());

            var bbox = BoundingBox.CreateBoundingBox(TestHelper.BerlinGeoCenter, 1000);

            var testModelVisitor = new TestModelVisitor();

            var elementManager = new ElementManager();

            // ACT
            elementManager.VisitBoundingBox(bbox, dataSource, new WayVisitor(testModelVisitor));

            // ASSERT
            Assert.AreEqual(1820, testModelVisitor.Ways.Count());
        }
    }
}