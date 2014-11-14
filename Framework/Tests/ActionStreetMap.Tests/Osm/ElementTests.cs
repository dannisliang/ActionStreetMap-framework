using System.IO;
using System.Linq;
using ActionStreetMap.Core;
using ActionStreetMap.Maps.Osm;
using ActionStreetMap.Maps.Osm.Data;
using ActionStreetMap.Maps.Osm.Entities;
using ActionStreetMap.Maps.UnitTests.Osm.Stubs;
using NUnit.Framework;

namespace ActionStreetMap.Maps.UnitTests.Osm
{
    [TestFixture]
    internal class ElementTests
    {
        private readonly GeoCoordinate defaultMapPoint = new GeoCoordinate(51.26371, 4.7854);

        [Test]
        public void CanGetElements()
        {
            // ARRANGE
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                var dataSource = new PbfElementSource(stream);

                var bbox = BoundingBox.CreateBoundingBox(defaultMapPoint, 500);

                // ACT
                var elements = dataSource.Get(bbox);

                // ASSERT
                Assert.AreEqual(6834, elements.Count());
            }
        }

        [Test]
        public void CanGetRelations()
        {
            // ARRANGE
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                var dataSource = new PbfElementSource(stream);
                var bbox = BoundingBox.CreateBoundingBox(defaultMapPoint, 1000);

                // ACT
                var elements = dataSource.Get(bbox);

                var relations = elements.OfType<Relation>().Select(element => element);

                // ASSERT
                Assert.AreEqual(39, relations.Count());
            }
        }

        [Test]
        public void CanFillBoundingBox()
        {
            // ARRANGE
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                var dataSource = new PbfElementSource(stream);
                var bbox = BoundingBox.CreateBoundingBox(defaultMapPoint, 300);

                var visitor = new CountableElementVisitor();

                var elementManager = new ElementManager();

                // ACT
                elementManager.VisitBoundingBox(bbox, dataSource, visitor);

                // ASSERT
                Assert.AreEqual(2840, visitor.Elements.Count);
            }
        }

        [Test]
        public void CanFillSmallBoundingBox()
        {
            // ARRANGE
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                var dataSource = new PbfElementSource(stream);

                var bbox = BoundingBox.CreateBoundingBox(new GeoCoordinate(51.26371, 4.7853), 10);

                var visitor = new CountableElementVisitor();

                var elementManager = new ElementManager();

                // ACT
                elementManager.VisitBoundingBox(bbox, dataSource, visitor);

                // ASSERT
                Assert.AreEqual(34, visitor.Elements.Count);
            }
        }

        [Test]
        public void CanFillOneBuildingBoundingBox()
        {
            // ARRANGE
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                var dataSource = new PbfElementSource(stream);

                var bbox = BoundingBox.CreateBoundingBox(new GeoCoordinate(51.26371, 4.7853), 5);

                var visitor = new CountableElementVisitor();

                var elementManager = new ElementManager();

                // ACT
                elementManager.VisitBoundingBox(bbox, dataSource, visitor);

                // ASSERT
                Assert.AreEqual(8, visitor.Elements.Count);
            }
        }

        [Test(Description = "Checks whether ElementManager is able to fully load elements between bbox borders")]
        public void CanDynamicallyLoadCrossBorderElements()
        {
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                // ARRANGE
                var dataSource = new PbfElementSource(stream);

                var bbox = BoundingBox.CreateBoundingBox(new GeoCoordinate(51.26371, 4.7853), 5);

                var visitor = new CountableElementVisitor();

                var elementManager = new ElementManager();

                // ACT
                elementManager.VisitBoundingBox(bbox, dataSource, visitor);

                // ASSERT
                var elements1 = visitor.Elements;
                // check test preconditions
                const int testWayId = 88246839;
                //const int nodeIdToBeResolved = 1025253741;
                var way = elements1.First(e => e.Id == testWayId) as Way;
                Assert.IsNotNull(way);
                Assert.AreEqual(testWayId, way.Id);
                Assert.AreEqual(8, way.Nodes.Count);

                /*// two cause it's start point (end point equals start point):
                
                Assert.AreEqual(2, way.Nodes.Count(n => n.Id == nodeIdToBeResolved));
                Assert.AreEqual(8, way.NodeIds.Count);

                // TODO we need sort nodes in the same order like nodeIds property before create polygon!
                
                // Act
                visitor = new CountableElementVisitor();

                // this bbox contains only one point for this way and it's not the same as nodeIdToBeResolved
                bbox = BoundingBox.CreateBoundingBox(new GeoCoordinate(51.263635, 4.785224), 8);
                elementManager.VisitBoundingBox(bbox, dataSource, visitor);

                var elements2 = visitor.Elements.ToList();
                way = elements2.First(e => e.Id == testWayId) as Way;
                Assert.IsNotNull(way);
                Assert.AreEqual(testWayId, way.Id);
                Assert.AreEqual(8, way.NodeIds.Count);
                Assert.AreEqual(3, way.Nodes.Count, "Unable to memorize resolved nodes for uncompleted polygon which crosses bbox border");
                Assert.AreEqual(2, way.Nodes.Count(n => n.Id == nodeIdToBeResolved));*/
            }
        }
    }
}