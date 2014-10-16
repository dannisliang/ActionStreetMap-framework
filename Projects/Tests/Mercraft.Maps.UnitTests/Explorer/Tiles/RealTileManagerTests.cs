using Mercraft.Core;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Dependencies.Interception.Behaviors;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles
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
            componentRoot.RunGame(new GeoCoordinate(52.5219497, 13.3688725));

            // ACT
            var tileLoader = container.Resolve<IPositionListener>() as TileManager;
            tileLoader.OnMapPositionChanged(new MapPoint(0, 0));
            logger.Stop();

            // ASSERT
            Assert.IsNotNull(tileLoader);
            Assert.AreEqual(1, tileLoader.Count);

            Assert.Less(logger.Seconds, 3, "Loading took to long");
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
            tileLoader.OnMapPositionChanged(new MapPoint(0, 0));

            // ASSERT
            Assert.IsNotNull(tileLoader);
            Assert.IsTrue(tileLoader.GetType().FullName.Contains("Mercraft.Dynamics"));
        }
    }
}
