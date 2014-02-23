
using System.IO;
using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Visitors;
using Mercraft.Models;
using Mercraft.Models.Scene;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Osm
{
    [TestFixture]
    public class BuildingTests
    {
        [Test]
        public void CanProcessBuildings()
        {
            using (Stream stream = new FileInfo(TestHelper.TestXmlFilePath).OpenRead())
            {
                var dataSource = MemoryDataSource.CreateFromXmlStream(stream);

                var bbox = BoundingBox.CreateBoundingBox(new MapPoint(52.529814, 13.388015), 0.2);

                var scene = new CountableScene();

                var elementManager = new ElementManager(new BuildingVisitor(scene));

                elementManager.VisitBoundingBox(dataSource, bbox);

                Assert.AreEqual(30, scene.Buildings.Count);

                DumpScene(scene);

            }
        }

        [Test]
        public void CanProcessLargerArea()
        {
            using (Stream stream = new FileInfo(TestHelper.TestPbfFilePath).OpenRead())
            {
                var dataSource = MemoryDataSource.CreateFromPbfStream(stream);

                var bbox = BoundingBox.CreateBoundingBox(new MapPoint(51.26371, 4.7854), 1);

                var scene = new CountableScene();

                var elementManager = new ElementManager(new BuildingVisitor(scene));

                elementManager.VisitBoundingBox(dataSource, bbox);

                Assert.AreEqual(1453, scene.Buildings.Count);
            }
        }

        private void DumpScene(CountableScene scene)
        {
            using (var file = File.CreateText(@"f:\scene.txt"))
            {
                file.WriteLine("BUILDINGS:");
                for (int i = 0; i < scene.Buildings.Count; i++)
                {
                    var building = scene.Buildings[i];
                    file.WriteLine("\tBuilding {0}", (i+1));
                    var lineOffset = "\t\t";
                    file.WriteLine("{0}Tags:", lineOffset);
                    foreach (var tag in building.Tags)
                    {
                        file.WriteLine("{0}\t{1}:{2}",lineOffset, tag.Key, tag.Value);
                    }
                    file.WriteLine("{0}Points:", lineOffset);
                    foreach (var point in building.Points)
                    {
                        file.WriteLine("{0}\t{1},{2}", lineOffset, point.Latitude, point.Longitude);
                    }
                }
            }
        }
    }
}
