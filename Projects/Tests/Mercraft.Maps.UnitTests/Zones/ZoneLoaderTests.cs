
using System;
using System.Security;
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
        // NOTE unfortuanelly, normal flow of test is broken once GameObject instance is touched
        [Test]
        [ExpectedException(typeof(SecurityException))]
        public void CanLoadZoneDynamically()
        {
            var container = new Container();
            var root = new GameRunner(container, new ConfigSettings(TestHelper.ConfigRootFile));
            root.RunGame(TestHelper.BerlinGeoCenter);

            var zoneLoader = container.Resolve<IPositionListener>() as TestZoneLoader;

            Assert.IsNotNull(zoneLoader);
            Assert.AreEqual(1, zoneLoader.ZoneCollection.Count);
        }
    }
}
