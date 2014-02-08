using System.IO;
using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.UnitTests.Stubs;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Osm
{
    [TestFixture]
    class ElementTests
    {
        [Test]
        public void CanGetOsmGeo()
        {
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                 var dataSource = MemoryDataSource.CreateFromPbfStream(stream);
                 var box = TestHelper.CreateBox();

                 var osmGeos = dataSource.Get(box, null);

                 Assert.AreEqual(3856, osmGeos.Count);
            }           
        }

        [Test]
        public void CanFillBoundingBox()
        {
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                var dataSource = MemoryDataSource.CreateFromPbfStream(stream);
                var box = TestHelper.CreateBox();

                var visitor = new CountableElementVisitor();

                var elementManager = new ElementManager(visitor);

                elementManager.FillBoundingBox(dataSource, box);

                Assert.AreEqual(3856, visitor.Elements.Count);
            }               
        }

        [Test]
        public void CanFillSmallBoundingBox()
        {
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                var dataSource = MemoryDataSource.CreateFromPbfStream(stream);
                var box = TestHelper.CreateBox(200, 200, 51.26371, 4.7853, 19);

                var visitor = new CountableElementVisitor();

                var elementManager = new ElementManager(visitor);

                elementManager.FillBoundingBox(dataSource, box);

                Assert.AreEqual(36, visitor.Elements.Count);
            }
        }

        [Test]
        public void CanFillOneBuildingBoundingBox()
        {
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                var dataSource = MemoryDataSource.CreateFromPbfStream(stream);

                var box = TestHelper.CreateBox(30, 30, 51.26371, 4.7853, 19);

                var visitor = new CountableElementVisitor();

                var elementManager = new ElementManager(visitor);

                elementManager.FillBoundingBox(dataSource, box);

                Assert.AreEqual(2, visitor.Elements.Count);
            }
        }


        #region Helpers

      

        #endregion
    }
}
