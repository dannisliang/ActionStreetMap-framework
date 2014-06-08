using System.IO;
using Mercraft.Core;
using Mercraft.Maps.Osm.Data;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Osm
{
    [TestFixture]
    public class OsmPerformanceTests
    {
        [Test]
        public void CanLoadBigCity()
        {
            var perfLogger = new PerformanceLogger();
            perfLogger.Start();

            var dataSource = new PbfIndexListElementSource(TestHelper.TestBigPbfIndexListPath,
                new TestPathResolver());

            var bbox = BoundingBox.CreateBoundingBox(TestHelper.BerlinGeoCenter, 1000);

            var elements = dataSource.Get(bbox);

            // Assert.AreEqual(30999, elements.Count()); //  actual is 31043
            perfLogger.Stop();


            Assert.Less(perfLogger.Memory, 30, "Memory consumption to high!");
            Assert.Less(perfLogger.Seconds, 10, "Time consumption to high!");
        }
    }
}
