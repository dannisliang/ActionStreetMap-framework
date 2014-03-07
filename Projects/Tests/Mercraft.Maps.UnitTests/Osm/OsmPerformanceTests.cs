﻿using System;
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
        [Ignore]
        [Test]
        public void CanLoadBigCity()
        {
            var perfLogger = new PerformanceLogger();
            perfLogger.Start();

            using (Stream stream = new FileInfo(TestHelper.TestBigPbfFilePath).OpenRead())
            {
                var dataSource = new PbfElementSource(stream);

                var bbox = BoundingBox.CreateBoundingBox(TestHelper.BerlinGeoCenter, 1000);

                var osmGeos = dataSource.Get(bbox);

                Assert.AreEqual(30999, osmGeos.Count()); //  actual is 31043
                perfLogger.Stop();
            }  

            Assert.Less(perfLogger.Memory, 200, "Memory consumption to high!");
            Assert.Less(perfLogger.Seconds, 20, "Time consumption to high!");
        }
    }
}