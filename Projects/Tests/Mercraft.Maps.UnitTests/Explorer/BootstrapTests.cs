using System.Linq;
using Mercraft.Core.Tiles;
using Mercraft.Explorer;
using NUnit.Framework;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Explorer
{
    [TestFixture]
    public class BootstrapTests
    {
        [Test]
        public void CanUseComponentRoot()
        {
            var componentRoot = new ComponentRoot();

            var tileProvider = componentRoot.Container.Resolve<TileProvider>();
            var tile = tileProvider.GetTile(new Vector2(0, 0));

            var buildings = tile.Scene.Buildings.ToList();
            Assert.AreEqual(30, buildings.Count);
        }
    }
}
