using System.IO;
using System.Linq;
using ActionStreetMap.Core;
using ActionStreetMap.Explorer.Infrastructure;
using ActionStreetMap.Osm;
using ActionStreetMap.Osm.Data;
using ActionStreetMap.Osm.Visitors;
using ActionStreetMap.Core.Scene;
using NUnit.Framework;

namespace ActionStreetMap.Tests.Osm
{
    [TestFixture]
    public class BuildingTests
    {
        [Test]
        public void CanProcessBuildingsXml()
        {
            // ARRANGE
            using (Stream stream = new FileInfo(TestHelper.TestXmlFilePath).OpenRead())
            {
                var dataSource = new XmlElementSource(stream);

                var bbox = BoundingBox.CreateBoundingBox(TestHelper.BerlinGeoCenter, 200);

                var scene = new TestModelVisitor();

                var elementManager = new ElementManager();

                // ACT
                elementManager.VisitBoundingBox(bbox, dataSource, new WayVisitor(scene, new ObjectPool()));

                // ASSERT
                Assert.AreEqual(36, scene.Areas.Count());
                Assert.AreEqual(30, scene.Areas.Count(a => a.Tags.Any(t => t.Key.Contains("building"))));
            }
        }

        [Test]
        public void CanProcessLargerAreaPbf()
        {
            // ARRANGE
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                var dataSource = new PbfElementSource(stream);

                var bbox = BoundingBox.CreateBoundingBox(new GeoCoordinate(51.26371, 4.7854), 1000);

                var scene = new TestModelVisitor();

                var elementManager = new ElementManager();

                // ACT
                elementManager.VisitBoundingBox(bbox, dataSource, new WayVisitor(scene, new ObjectPool()));

                // ASSERT
                Assert.AreEqual(1696, scene.Areas.Count());
                Assert.AreEqual(1453, scene.Areas.Count(a => a.Tags.Any(t => t.Key.Contains("building"))));
            }
        }
    }
}