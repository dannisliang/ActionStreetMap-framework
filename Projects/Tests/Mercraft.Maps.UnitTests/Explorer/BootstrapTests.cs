using System.Linq;
using Mercraft.Core.Scene;
using Mercraft.Explorer;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Explorer
{
    [TestFixture]
    public class BootstrapTests
    {
        [Test(Description = "Tests whether we can run game using default bootstrappers defined in test.config")]
        public void CanUseComponentRoot()
        {
            // Act
            var componentRoot = new GameRunner(TestHelper.ConfigRootFile);
            componentRoot.RunGame(TestHelper.BerlinGeoCenter);
        }

        [Test]
        public void CanResolveBuildingModelVisitors()
        {
            // Arrange
            var container = new Container();
            var root = new GameRunner(container, new ConfigSettings(TestHelper.ConfigRootFile));
            root.RunGame(TestHelper.BerlinGeoCenter);

            // Act
            var modelVisitors = container.ResolveAll<ISceneModelVisitor>()
                .ToList();

            // Assert
            Assert.AreEqual(2, modelVisitors.Count());
        }
    }
}
