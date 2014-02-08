
using System.IO;
using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Visitors;
using Mercraft.Maps.UnitTests.Stubs;
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

                var box = TestHelper.CreateBox(500, 500, 52.529814, 13.388015, 16);

                var scene = new CountableScene();

                var elementManager = new ElementManager(new BuildingVisitor(scene));

                elementManager.FillBoundingBox(dataSource, box);

                Assert.AreEqual(30, scene.Buildings.Count);
            }
        }
    }
}
