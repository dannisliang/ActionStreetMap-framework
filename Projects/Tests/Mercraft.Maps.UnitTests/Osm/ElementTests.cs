using System.IO;
using System.Linq;
using Mercraft.Maps.Osm;
using Mercraft.Core;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.UnitTests.Osm.Stubs;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Osm
{
    [TestFixture]
    class ElementTests
    {
        private GeoCoordinate defaultMapPoint = new GeoCoordinate(51.26371, 4.7854);
        [Test]
        public void CanGetElements()
        {
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                var dataSource = new PbfElementSource(stream);

                var bbox = BoundingBox.CreateBoundingBox(defaultMapPoint, 500);

                var osmGeos = dataSource.Get(bbox);

                Assert.AreEqual(6025, osmGeos.Count());
            }           
        }


        [Test]
        public void CanFillBoundingBox()
        {
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                var dataSource = new PbfElementSource(stream);
                var bbox = BoundingBox.CreateBoundingBox(defaultMapPoint, 300);

                var visitor = new CountableElementVisitor();

                var elementManager = new ElementManager();

                elementManager.VisitBoundingBox(bbox, dataSource, visitor);

                Assert.AreEqual(2566, visitor.Elements.Count);
            }               
        }

        [Test]
        public void CanFillSmallBoundingBox()
        {
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                var dataSource = new PbfElementSource(stream);

                var bbox = BoundingBox.CreateBoundingBox(new GeoCoordinate(51.26371, 4.7853), 10);

                var visitor = new CountableElementVisitor();

                var elementManager = new ElementManager();

                elementManager.VisitBoundingBox(bbox, dataSource, visitor);

                Assert.AreEqual(21, visitor.Elements.Count);
            }
        }

        [Test]
        public void CanFillOneBuildingBoundingBox()
        {
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                var dataSource = new PbfElementSource(stream);

                var bbox = BoundingBox.CreateBoundingBox(new GeoCoordinate(51.26371, 4.7853), 5);

                var visitor = new CountableElementVisitor();

                var elementManager = new ElementManager();

                elementManager.VisitBoundingBox(bbox, dataSource, visitor);

                Assert.AreEqual(2, visitor.Elements.Count);
            }
        }


        #region Helpers

      

        #endregion
    }
}
