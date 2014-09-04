using Mercraft.Core;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Infrastructure.Config;
using Mercraft.Maps.UnitTests.Zones.Stubs;
using Moq;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Zones
{
    [TestFixture]
    internal class TileProviderTests
    {
        private const float Size = 50;
        private const float Half = Size/2;
        private const float Offset = 5;

        [Test]
        public void CanMoveLeft()
        {
            // ARRANGE
            var provider = GetProvider();
            var center = new GeoCoordinate(52, 13);

            // ACT & ASSERT
            var tileCenter = provider.GetTile(new MapPoint(0, 0), center);

            // left tile
            var tile = CanLoadTile(provider, tileCenter, center,
                new MapPoint(-(Half - Offset - 1), 0),
                new MapPoint(-(Half - Offset), 0),
                new MapPoint(-(Half * 2 - Offset - 1), 0), 0);

            Assert.AreEqual(tile.MapCenter, new MapPoint(-Size, 0));
        }

        [Test]
        public void CanMoveRight()
        {
            // ARRANGE
            var provider = GetProvider();
            var center = new GeoCoordinate(52, 13);

            // ACT & ASSERT
            var tileCenter = provider.GetTile(new MapPoint(0, 0), center);

            // right tile
            var tile = CanLoadTile(provider, tileCenter, center,
                new MapPoint(Half - Offset - 1, 0),
                new MapPoint(Half - Offset, 0),
                new MapPoint(Half * 2 - Offset - 1, 0), 0);

            Assert.AreEqual(tile.MapCenter, new MapPoint(Size, 0));
        }

        [Test]
        public void CanMoveTop()
        {
            // ARRANGE
            var provider = GetProvider();
            var center = new GeoCoordinate(52, 13);

            // ACT & ASSERT
            var tileCenter = provider.GetTile(new MapPoint(0, 0), center);

            // top tile
            var tile = CanLoadTile(provider, tileCenter, center,
                new MapPoint(0, Half - Offset - 1),
                new MapPoint(0, Half - Offset),
                new MapPoint(0, Half * 2 - Offset - 1), 0);

            Assert.AreEqual(tile.MapCenter, new MapPoint(0, Size));
        }

        [Test]
        public void CanMoveBottom()
        {
            // ARRANGE
            var provider = GetProvider();
            var center = new GeoCoordinate(52, 13);

            // ACT & ASSERT
            var tileCenter = provider.GetTile(new MapPoint(0, 0), center);

            // bottom tile
            var tile = CanLoadTile(provider, tileCenter, center,
                new MapPoint(0, -(Half - Offset - 1)),
                new MapPoint(0, -(Half - Offset)),
                new MapPoint(0, -(Half * 2 - Offset - 1)), 0);

            Assert.AreEqual(tile.MapCenter, new MapPoint(0, -Size));
        }

        [Test]
        public void CanMoveArround()
        {
            // ARRANGE
            var provider = GetProvider();
            var center = new GeoCoordinate(52, 13);

            // ACT & ASSERT
            var tileCenter = provider.GetTile(new MapPoint(0, 0), center);

            // left tile
            CanLoadTile(provider, tileCenter, center,
                new MapPoint(-(Half - Offset - 1), 0),
                new MapPoint(-(Half - Offset), 0),
                new MapPoint(-(Half*2 - Offset - 1), 0), 0);

            // right tile
            CanLoadTile(provider, tileCenter, center,
                new MapPoint(Half - Offset - 1, 0),
                new MapPoint(Half - Offset, 0),
                new MapPoint(Half*2 - Offset - 1, 0), 1);

            // top tile
            CanLoadTile(provider, tileCenter, center,
                new MapPoint(0, Half - Offset - 1),
                new MapPoint(0, Half - Offset),
                new MapPoint(0, Half*2 - Offset - 1), 2);

            // bottom tile
            CanLoadTile(provider, tileCenter, center,
                new MapPoint(0, -(Half - Offset - 1)),
                new MapPoint(0, -(Half - Offset)),
                new MapPoint(0, -(Half*2 - Offset - 1)), 3);
        }

        private TileProvider GetProvider()
        {
            var sceneBuilderMock = new Mock<ISceneBuilder>();
            sceneBuilderMock
                .Setup(s => s.Build(It.IsAny<BoundingBox>())).Returns(new MapScene
                {
                    Canvas = new Canvas()
                });

            var configMock = new Mock<IConfigSection>();
            configMock.Setup(c => c.GetFloat("@size")).Returns(Size);
            configMock.Setup(c => c.GetFloat("@offset")).Returns(Offset);

            var provider = new TileProvider(sceneBuilderMock.Object, new MessageBus());
            provider.Configure(configMock.Object);

            return provider;
        }

        private Tile CanLoadTile(TileProvider provider, Tile tileCenter, GeoCoordinate center,
            MapPoint first, MapPoint second, MapPoint third, int tileCount)
        {
            // this shouldn't load new tile
            var tileCenterTest1 = provider.GetTile(first, center);
            Assert.AreSame(tileCenter, tileCenterTest1);
            Assert.AreEqual(++tileCount, provider.TileCount);
            Assert.AreEqual(tileCenterTest1.MapCenter, new MapPoint(0, 0));

            // this force to load new tile
            var tileNext = provider.GetTile(second, center);
            Assert.AreNotSame(tileCenter, tileNext);
            Assert.AreEqual(++tileCount, provider.TileCount);

            // this shouldn't load new tile
            var tileNextTest = provider.GetTile(third, center);
            Assert.AreSame(tileNext, tileNextTest);
            Assert.AreEqual(tileCount, provider.TileCount);

            return tileNext;
        }
    }
}