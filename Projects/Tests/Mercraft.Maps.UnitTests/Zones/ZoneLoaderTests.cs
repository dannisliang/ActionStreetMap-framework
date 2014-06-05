using System;
using System.Linq;
using Mercraft.Core;
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
            var logger = new PerformanceLogger();
            logger.Start();
            var container = new Container();
            var pathResolver = TestHelper.GetPathResolver();
            container.RegisterInstance(typeof(IPathResolver), pathResolver);
            var componentRoot = new GameRunner(container,
                new ConfigSettings(TestHelper.ConfigTestRootFile, pathResolver));
            componentRoot.RunGame(TestHelper.BerlinInvalidenStr);

            var zoneLoader = container.Resolve<IPositionListener>() as TestZoneLoader;

            logger.Stop();

            Assert.IsNotNull(zoneLoader);
            Assert.AreEqual(1, zoneLoader.ZoneCollection.Count);

            Assert.Less(logger.Seconds, 10, "Loading took to long");
            // NOTE However, we only check memory which is used after GC
            Assert.Less(logger.Memory, 50, "Memory consumption to hight!");
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ShouldUnloadTile()
        {
            var container = new Container();
            var pathResolver = TestHelper.GetPathResolver();
            container.RegisterInstance(typeof(IPathResolver), pathResolver);
            var componentRoot = new GameRunner(container,
                new ConfigSettings(TestHelper.ConfigTestRootFile, pathResolver));
            componentRoot.RunGame(TestHelper.BerlinGeoCenter);

            var zoneLoader = container.Resolve<IPositionListener>() as TestZoneLoader;

            Assert.IsNotNull(zoneLoader.ZoneCollection.Single().Tile.Scene.Areas);
        }
    }
}
