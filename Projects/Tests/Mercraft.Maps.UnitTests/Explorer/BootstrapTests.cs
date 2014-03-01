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
            // Act
            var componentRoot = new ComponentRoot(TestHelper.ConfigRootFile);

            componentRoot.RunGame(TestHelper.BerlinGeoCenter);

            // Assert

            //var tileProvider = componentRoot.Container.Resolve<TileProvider>();
            //var tile = tileProvider.GetTile(new Vector2(0, 0));

           // var buildings = tile.Scene.Buildings.ToList();
           // Assert.AreEqual(30, buildings.Count);
        }
    }
}
