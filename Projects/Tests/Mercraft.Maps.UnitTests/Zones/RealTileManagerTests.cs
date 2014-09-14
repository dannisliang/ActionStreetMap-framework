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
    public class RealTileManagerTests
    {
        [Test]
        public void CanLoadTileDynamically()
        {
            // ARRANGE
            var logger = new PerformanceLogger();
            logger.Start();
            var container = new Container();
            var componentRoot = TestHelper.GetGameRunner(container);
            componentRoot.RunGame(new GeoCoordinate(52.52227, 13.40925));

            // ACT
            var tileLoader = container.Resolve<IPositionListener>() as TileManager;

            logger.Stop();

            // ASSERT
            Assert.IsNotNull(tileLoader);
            Assert.AreEqual(1, GetTiles(tileLoader).Count());

            Assert.Less(logger.Seconds, 5, "Loading took to long");
            // NOTE However, we only check memory which is used after GC
            Assert.Less(logger.Memory, 20, "Memory consumption is to hight!");
        }

        [Test]
        public void CanLoadTileWithProxy()
        {
            // ARRANGE
            var container = new Container();

            container.AllowProxy = true;
            container.AutoGenerateProxy = true;
            container.AddGlobalBehavior(new ExecuteBehavior());

            var componentRoot = TestHelper.GetGameRunner(container);
            componentRoot.RunGame(TestHelper.BerlinInvalidenStr);

            // ACT
            var tileLoader = container.Resolve<IPositionListener>();

            // ASSERT
            Assert.IsNotNull(tileLoader);
            Assert.IsTrue(tileLoader.GetType().FullName.Contains("Mercraft.Dynamics"));
        }

        private IEnumerable<Tile> GetTiles(TileManager tileManager)
        {
            var property = typeof(TileManager).GetProperty("Tiles", BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.GetProperty);
            return (property.GetValue(tileManager, null) as HashSet<Tile>).AsEnumerable();
        }
    }
}
