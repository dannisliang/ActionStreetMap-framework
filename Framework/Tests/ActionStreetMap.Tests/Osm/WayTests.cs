using System.Linq;
using ActionStreetMap.Core;
using ActionStreetMap.Explorer.Infrastructure;
using ActionStreetMap.Infrastructure.Diagnostic;
using ActionStreetMap.Maps.Osm;
using ActionStreetMap.Maps.Osm.Data;
using ActionStreetMap.Maps.Osm.Visitors;
using ActionStreetMap.Core.Scene;
using NUnit.Framework;

namespace ActionStreetMap.Maps.UnitTests.Osm
{
    [TestFixture]
    public class WayTests
    {
        [Test]
        public void CanFindWays()
        {
            // ARRANGE
            var dataSource = new PbfIndexListElementSource(TestHelper.TestBigPbfIndexListPath,
                TestHelper.GetFileSystemService(), new DefaultTrace());

            var bbox = BoundingBox.CreateBoundingBox(TestHelper.BerlinGeoCenter, 1000);

            var testModelVisitor = new TestModelVisitor();

            var elementManager = new ElementManager();

            // ACT
            elementManager.VisitBoundingBox(bbox, dataSource, new WayVisitor(testModelVisitor, new ObjectPool()));

            // ASSERT
            Assert.AreEqual(1820, testModelVisitor.Ways.Count());
        }
    }
}