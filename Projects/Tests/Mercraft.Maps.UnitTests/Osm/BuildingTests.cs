
using System.IO;
using System.Linq;
using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Visitors;
using Mercraft.Core;
using Mercraft.Core.Scene;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Osm
{
    [TestFixture]
    public class BuildingTests
    {
        [Test]
        public void CanProcessBuildingsXml()
        {
            using (Stream stream = new FileInfo(TestHelper.TestXmlFilePath).OpenRead())
            {
                var dataSource = new XmlElementSource(stream);

                var bbox = BoundingBox.CreateBoundingBox(TestHelper.SmallPbfFileCenter, 200);

                var scene = new MapScene();

                var elementManager = new ElementManager();

                elementManager.VisitBoundingBox(bbox, dataSource, new WayVisitor(scene));

                Assert.AreEqual(36, scene.Areas.Count());
                Assert.AreEqual(30, scene.Areas.Count(a => a.Tags.Any(t => t.Key.Contains("building"))));
            }
        }

        [Test]
        public void CanProcessLargerAreaPbf()
        {
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                var dataSource = new PbfElementSource(stream);

                var bbox = BoundingBox.CreateBoundingBox(new GeoCoordinate(51.26371, 4.7854), 1000);

                var scene = new MapScene();

                var elementManager = new ElementManager();

                elementManager.VisitBoundingBox(bbox, dataSource, new WayVisitor(scene));

                Assert.AreEqual(1696, scene.Areas.Count());
                Assert.AreEqual(1453, scene.Areas.Count(a => a.Tags.Any(t => t.Key.Contains("building"))));
            }
        }
    }
}
