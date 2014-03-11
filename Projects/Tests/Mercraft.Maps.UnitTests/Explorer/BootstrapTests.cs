using System.Linq;
using Mercraft.Core.Scene;
using Mercraft.Explorer;
using Mercraft.Explorer.Meshes;
using Mercraft.Explorer.Render;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Explorer
{
    [TestFixture]
    public class BootstrapTests
    {
        [Test(Description = "Tests whether we can run game using default bootstrappers defined in test.config")]
        public void CanUseGameRunner()
        {
            // Act
            var gameRunner = new GameRunner(TestHelper.ConfigRootFile);
            gameRunner.RunGame(TestHelper.BerlinGeoCenter);
        }

        [Test]
        public void CanResolveModelVisitors()
        {
            // Arrange
            var container = new Container();
            var root = new GameRunner(container, new ConfigSettings(TestHelper.ConfigRootFile));

            // Act
            var modelVisitors = container.ResolveAll<ISceneModelVisitor>().ToList();

            // Assert
            Assert.AreEqual(2, modelVisitors.Count());
        }

        [Test]
        public void CanResolveMeshRenders()
        {
            // Arrange
            var container = new Container();
            var root = new GameRunner(container, new ConfigSettings(TestHelper.ConfigRootFile));

            // Act
            var renders = container.ResolveAll<IMeshRenderer>().ToList();

            // Assert
            Assert.AreEqual(1, renders.Count());
        }

        [Test]
        public void CanResolveMeshBuilders()
        {
            // Arrange
            var container = new Container();
            var root = new GameRunner(container, new ConfigSettings(TestHelper.ConfigRootFile));

            // Act
            var builders = container.ResolveAll<IMeshBuilder>().ToList();

            // Assert
            Assert.AreEqual(1, builders.Count());
        }
    }
}
