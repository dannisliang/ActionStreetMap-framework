using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Tiles;
using Mercraft.Explorer;
using Mercraft.Infrastructure.Config;
using NUnit.Framework;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Explorer
{
    [TestFixture]
    public class BootstrapTests
    {
        [Test(Description = "Tests whether we can run game using default bootstrappers defined in test.config")]
        public void CanUseComponentRoot()
        {
            // Arrange
            var center = new GeoCoordinate(52.529814, 13.388015);

            // Act
            var config = new ConfigSettings("test.config");
            var componentRoot = new ComponentRoot(config);

            componentRoot.RunGame(center);

            // Assert

            //var tileProvider = componentRoot.Container.Resolve<TileProvider>();
            //var tile = tileProvider.GetTile(new Vector2(0, 0));

           // var buildings = tile.Scene.Buildings.ToList();
           // Assert.AreEqual(30, buildings.Count);
        }
    }
}
