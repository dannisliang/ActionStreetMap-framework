using System.IO;
using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.UnitTests.Stubs;
using Mercraft.Models;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Osm
{
    [TestFixture]
    class ElementTests
    {
        private MapPoint defaultMapPoint = new MapPoint(51.26371, 4.7854);
        [Test]
        public void CanGetElements()
        {
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                var dataSource = MemoryDataSource.CreateFromPbfStream(stream);

                var bbox = BoundingBox.GetBoundingBox(defaultMapPoint, 0.5);

                var osmGeos = dataSource.Get(bbox, null);

                Assert.AreEqual(6025, osmGeos.Count);
            }           
        }

        [Test]
        public void CanFillBoundingBox()
        {
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                var dataSource = MemoryDataSource.CreateFromPbfStream(stream);
                var bbox = BoundingBox.GetBoundingBox(defaultMapPoint, 0.3);

                var visitor = new CountableElementVisitor();

                var elementManager = new ElementManager(visitor);

                elementManager.FillScene(dataSource, bbox);

                Assert.AreEqual(2566, visitor.Elements.Count);
            }               
        }

        [Test]
        public void CanFillSmallBoundingBox()
        {
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                var dataSource = MemoryDataSource.CreateFromPbfStream(stream);

                var bbox = BoundingBox.GetBoundingBox(new MapPoint(51.26371, 4.7853), 0.01);

                var visitor = new CountableElementVisitor();

                var elementManager = new ElementManager(visitor);

                elementManager.FillScene(dataSource, bbox);

                Assert.AreEqual(21, visitor.Elements.Count);
            }
        }

        [Test]
        public void CanFillOneBuildingBoundingBox()
        {
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                var dataSource = MemoryDataSource.CreateFromPbfStream(stream);

                var bbox = BoundingBox.GetBoundingBox(new MapPoint(51.26371, 4.7853), 0.005);

                var visitor = new CountableElementVisitor();

                var elementManager = new ElementManager(visitor);

                elementManager.FillScene(dataSource, bbox);

                Assert.AreEqual(2, visitor.Elements.Count);
            }
        }


        #region Helpers

      

        #endregion
    }
}
