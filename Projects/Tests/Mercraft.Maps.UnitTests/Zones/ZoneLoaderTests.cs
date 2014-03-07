
using Mercraft.Core;
using Mercraft.Explorer;
using Mercraft.Maps.UnitTests.Zones.Stubs;
using NUnit.Framework;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Zones
{
    [TestFixture]
    public class ZoneLoaderTests
    {
        [Test]
        public void CanLoadZoneDynamically()
        {
            var root = new GameRunner(TestHelper.ConfigRootFile);
            root.RunGame(TestHelper.BerlinGeoCenter);

            var zoneLoader = root.Container.Resolve<IPositionListener>() as TestZoneLoader;

            Assert.IsNotNull(zoneLoader);
            Assert.AreEqual(1, zoneLoader.ZoneCollection.Count);
        }

        [Ignore]
        [Test]
        public void CanLoadZonesDynamically()
        {
            var tileHalfSize = 500;
            var offset = 50;
            GameRunner root = new GameRunner(TestHelper.ConfigRootFile);
            root.RunGame(TestHelper.BerlinGeoCenter);
            var zoneLoader = root.Container.Resolve<IPositionListener>() as TestZoneLoader;

            // same zone
            root.OnMapPositionChanged(new Vector2(tileHalfSize - offset - 1, 0));
            Assert.AreEqual(1, zoneLoader.ZoneCollection.Count);

            // diff zone
            root.OnMapPositionChanged(new Vector2(tileHalfSize - offset, 0));
            Assert.AreEqual(2, zoneLoader.ZoneCollection.Count);

        }
    }
}
