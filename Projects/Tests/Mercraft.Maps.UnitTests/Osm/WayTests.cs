using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mercraft.Core;
using Mercraft.Core.Scene;
using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Visitors;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Osm
{
    [TestFixture]
    public class WayTests
    {
        [Test]
        public void CanFindWays()
        {
            using (Stream stream = new FileInfo(TestHelper.TestBigPbfFilePath).OpenRead())
            {
                var dataSource = new PbfElementSource(stream);

                var bbox = BoundingBox.CreateBoundingBox(TestHelper.BerlinGeoCenter, 1000);

                var scene = new MapScene();

                var elementManager = new ElementManager();

                elementManager.VisitBoundingBox(bbox, dataSource, new WayVisitor(scene));

                Assert.AreEqual(1174, scene.Ways.Count());
            }
        }
    }
}
