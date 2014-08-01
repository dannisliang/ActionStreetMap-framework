using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mercraft.Core;
using Mercraft.Core.Tiles;
using Mercraft.Core.Zones;
using Mercraft.Explorer;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.UnitTests.Zones.Stubs;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Zones
{
    [TestFixture]
    public class ZoneLoaderTests
    {
        [Test]
        public void CanLoadZoneDynamically()
        {
            // ARRANGE
            var logger = new PerformanceLogger();
            logger.Start();
            var container = new Container();
            var componentRoot = TestHelper.GetGameRunner(container);
            componentRoot.RunGame(TestHelper.BerlinInvalidenStr);

            // ACT
            var zoneLoader = container.Resolve<IPositionListener>() as ZoneLoader;

            logger.Stop();

            // ASSERT
            Assert.IsNotNull(zoneLoader);
            Assert.AreEqual(1, GetZones(zoneLoader).Count());

            Assert.Less(logger.Seconds, 15, "Loading took to long");
            // NOTE However, we only check memory which is used after GC
            Assert.Less(logger.Memory, 50, "Memory consumption to hight!");
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ShouldUnloadTile()
        {
            // ARRANGE
            var container = new Container();
            var componentRoot = TestHelper.GetGameRunner(container);
            componentRoot.RunGame(TestHelper.BerlinGeoCenter);

            // ACT
            var zoneLoader = container.Resolve<IPositionListener>() as ZoneLoader;

            // ASSERT
            Assert.IsNotNull(GetZones(zoneLoader).Single().Tile.Scene.Areas);
        }

        private IEnumerable<Zone> GetZones(ZoneLoader zoneLoader)
        {
            var property = typeof (ZoneLoader).GetProperty("Zones", BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.GetProperty);
            return (property.GetValue(zoneLoader, null) as Dictionary<Tile, Zone>).Values;
        }
    }
}
