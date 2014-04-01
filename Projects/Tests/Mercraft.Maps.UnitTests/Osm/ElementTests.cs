using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mercraft.Maps.Osm;
using Mercraft.Core;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Entities;
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

                var elements = dataSource.Get(bbox);

                Assert.AreEqual(6834, elements.Count());
            }           
        }

        [Test]
        public void CanGetRelations()
        {
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                var dataSource = new PbfElementSource(stream);
                var bbox = BoundingBox.CreateBoundingBox(defaultMapPoint, 1000);

                var elements = dataSource.Get(bbox);

                var relations = elements.OfType<Relation>().Select(element => element);

                Assert.AreEqual(39, relations.Count());

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

                Assert.AreEqual(2840, visitor.Elements.Count);
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

                Assert.AreEqual(34, visitor.Elements.Count);
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

                Assert.AreEqual(8, visitor.Elements.Count);
            }
        }

        [Test(Description = "Checks whether ElementManager is able to fully load elements between bbox borders")]
        public void CanDynamicallyLoadCrossBorderElements()
        {
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                // Arrange
                var dataSource = new PbfElementSource(stream);

                var bbox = BoundingBox.CreateBoundingBox(new GeoCoordinate(51.26371, 4.7853), 5);

                var visitor = new CountableElementVisitor();

                var elementManager = new ElementManager();
                elementManager.VisitBoundingBox(bbox, dataSource, visitor);

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
