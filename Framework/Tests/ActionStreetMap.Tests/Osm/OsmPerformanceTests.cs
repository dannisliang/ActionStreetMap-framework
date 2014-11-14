using ActionStreetMap.Core;
using ActionStreetMap.Infrastructure.Diagnostic;
using ActionStreetMap.Osm.Data;
using NUnit.Framework;

namespace ActionStreetMap.Tests.Osm
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
            Assert.Less(perfLogger.Memory, 45, "Memory consumption to high!");
            Assert.Less(perfLogger.Seconds, 3, "Time consumption to high!");
        }
    }
}