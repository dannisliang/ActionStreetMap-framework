using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mercraft.Core;
using Mercraft.Core.Tiles;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Dependencies.Interception.Behaviors;
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
            componentRoot.RunGame(TestHelper.BerlinHauptBanhoff);

            // ACT
            var zoneLoader = container.Resolve<IPositionListener>() as TileLoader;

            logger.Stop();

            // ASSERT
            Assert.IsNotNull(zoneLoader);
            Assert.AreEqual(1, GetZones(zoneLoader).Count());

            Assert.Less(logger.Seconds, 5, "Loading took to long");
            // NOTE However, we only check memory which is used after GC
            Assert.Less(logger.Memory, 20, "Memory consumption is to hight!");
        }

        [Test]
        public void CanLoadZoneWithProxy()
        {
            // ARRANGE
            var container = new Container();

            container.AllowProxy = true;
            container.AutoGenerateProxy = true;
            container.AddGlobalBehavior(new ExecuteBehavior());

            var componentRoot = TestHelper.GetGameRunner(container);
            componentRoot.RunGame(TestHelper.BerlinInvalidenStr);

            // ACT
            var zoneLoader = container.Resolve<IPositionListener>();

            // ASSERT
            Assert.IsNotNull(zoneLoader);
            Assert.IsTrue(zoneLoader.GetType().FullName.Contains("Mercraft.Dynamics"));
        }

        private IEnumerable<Tile> GetZones(TileLoader zoneLoader)
        {
            var property = typeof(TileLoader).GetProperty("Tiles", BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.GetProperty);
            return (property.GetValue(zoneLoader, null) as HashSet<Tile>).AsEnumerable();
        }
    }
}
