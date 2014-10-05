using Mercraft.Core;
using Mercraft.Core.Elevation;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Infrastructure.Config;
using Moq;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles
{
    [TestFixture]
    internal class TileManagerTests
    {
        private const float Size = 50;
        private const float Half = Size/2;
        private const float Offset = 5;

        [Test]
        public void CanMoveLeft()
        {
            // ARRANGE
            var provider = GetManager();
            var center = new MapPoint(0, 0);

            // ACT & ASSERT
            provider.OnMapPositionChanged(center);

            // left tile
            var tile = CanLoadTile(provider, provider.CurrentTile,
                new MapPoint(-(Half - Offset - 1), 0),
                new MapPoint(-(Half - Offset), 0),
                new MapPoint(-(Half * 2 - Offset - 1), 0), 0);

            Assert.AreEqual(tile.MapCenter, new MapPoint(-Size, 0));
        }

        [Test]
        public void CanMoveRight()
        {
            // ARRANGE
            var provider = GetManager();
            var center = new MapPoint(0, 0);

            // ACT & ASSERT
            provider.OnMapPositionChanged(center);

            // right tile
            var tile = CanLoadTile(provider, provider.CurrentTile,
                new MapPoint(Half - Offset - 1, 0),
                new MapPoint(Half - Offset, 0),
                new MapPoint(Half * 2 - Offset - 1, 0), 0);

            Assert.AreEqual(tile.MapCenter, new MapPoint(Size, 0));
        }

        [Test]
        public void CanMoveTop()
        {
            // ARRANGE
            var provider = GetManager();
            var center = new MapPoint(0, 0);

            // ACT & ASSERT
            provider.OnMapPositionChanged(center);

            // top tile
            var tile = CanLoadTile(provider, provider.CurrentTile,
                new MapPoint(0, Half - Offset - 1),
                new MapPoint(0, Half - Offset),
                new MapPoint(0, Half * 2 - Offset - 1), 0);

            Assert.AreEqual(tile.MapCenter, new MapPoint(0, Size));
        }

        [Test]
        public void CanMoveBottom()
        {
            // ARRANGE
            var provider = GetManager();
            var center = new MapPoint(0, 0);

            // ACT & ASSERT
            provider.OnMapPositionChanged(center);

            // bottom tile
            var tile = CanLoadTile(provider, provider.CurrentTile,
                new MapPoint(0, -(Half - Offset - 1)),
                new MapPoint(0, -(Half - Offset)),
                new MapPoint(0, -(Half * 2 - Offset - 1)), 0);

            Assert.AreEqual(tile.MapCenter, new MapPoint(0, -Size));
        }

       /* [Test]
        public void CanMoveArround()
        {
            // ARRANGE
            var provider = GetManager();
            var center = new MapPoint(0, 0);

            // ACT & ASSERT
            provider.OnMapPositionChanged(center);

            // left tile
            CanLoadTile(provider, provider.CurrentTile,
                new MapPoint(-(Half - Offset - 1), 0),
                new MapPoint(-(Half - Offset), 0),
                new MapPoint(-(Half*2 - Offset - 1), 0), 0);

            // right tile
            CanLoadTile(provider, provider.CurrentTile,
                new MapPoint(Half - Offset - 1, 0),
                new MapPoint(Half - Offset, 0),
                new MapPoint(Half*2 - Offset - 1, 0), 1);

            // top tile
            CanLoadTile(provider, provider.CurrentTile,
                new MapPoint(0, Half - Offset - 1),
                new MapPoint(0, Half - Offset),
                new MapPoint(0, Half*2 - Offset - 1), 2);

            // bottom tile
            CanLoadTile(provider, provider.CurrentTile,
                new MapPoint(0, -(Half - Offset - 1)),
                new MapPoint(0, -(Half - Offset)),
                new MapPoint(0, -(Half*2 - Offset - 1)), 3);
        }*/

        private TileManager GetManager()
        {
            var sceneBuilderMock = new Mock<ITileLoader>();
            var heightMapProvider = new HeightMapProvider(new Mock<IElevationProvider>().Object);

            var configMock = new Mock<IConfigSection>();
            configMock.Setup(c => c.GetFloat("@size")).Returns(Size);
            configMock.Setup(c => c.GetFloat("@offset")).Returns(Offset);

            var provider = new TileManager(sceneBuilderMock.Object, heightMapProvider, new MessageBus());
            provider.Configure(configMock.Object);

            return provider;
        }

        private Tile CanLoadTile(TileManager manager, Tile tileCenter,
            MapPoint first, MapPoint second, MapPoint third, int tileCount)
        {
            // this shouldn't load new tile
            manager.OnMapPositionChanged(first);
            Assert.AreSame(tileCenter, manager.CurrentTile);

            ++tileCount;

            // this force to load new tile
            manager.OnMapPositionChanged(second);
            var tileNext = manager.CurrentTile;
            Assert.AreNotSame(tileCenter, tileNext);
            Assert.AreEqual(++tileCount, manager.TileCount);

            // this shouldn't load new tile
            manager.OnMapPositionChanged(third);
            Assert.AreSame(tileNext, manager.CurrentTile);
            Assert.AreEqual(tileCount, manager.TileCount);

            return tileNext;
        }
    }
}