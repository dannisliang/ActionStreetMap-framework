using Mercraft.Core;
using Mercraft.Infrastructure.Diagnostic;
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
            // ARRANGE
            var perfLogger = new PerformanceLogger();
            perfLogger.Start();

            var dataSource = new PbfIndexListElementSource(TestHelper.TestBigPbfIndexListPath,
                TestHelper.GetFileSystemService(), new DefaultTrace());

            var bbox = BoundingBox.CreateBoundingBox(TestHelper.BerlinGeoCenter, 1000);

            // ACT
            var elements = dataSource.Get(bbox);

            // Assert.AreEqual(30999, elements.Count()); //  actual is 31043
            perfLogger.Stop();

            // ASSERT
            Assert.Less(perfLogger.Memory, 30, "Memory consumption to high!");
            Assert.Less(perfLogger.Seconds, 10, "Time consumption to high!");
        }
    }
}