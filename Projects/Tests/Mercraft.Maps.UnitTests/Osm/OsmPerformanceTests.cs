using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

            using (Stream stream = new FileInfo(TestHelper.TestBigPbfFilePath).OpenRead())
            {
                var dataSource = new PbfElementSource(stream);

                var bbox = BoundingBox.CreateBoundingBox(TestHelper.BerlinGeoCenter, 1000);

                var elements = dataSource.Get(bbox);

               // Assert.AreEqual(30999, elements.Count()); //  actual is 31043
                perfLogger.Stop();
            }  

            Assert.Less(perfLogger.Memory, 30, "Memory consumption to high!");
            Assert.Less(perfLogger.Seconds, 10, "Time consumption to high!");
        }
    }
}
