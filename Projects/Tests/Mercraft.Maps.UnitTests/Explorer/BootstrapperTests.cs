using System.Linq;
using Mercraft.Core.Scene;
using Mercraft.Explorer;
using Mercraft.Explorer.Builders;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Explorer
{
    [TestFixture]
    public class BootstrapperTests
    {
        [Test]
        public void CanResolveModelBuilders()
        {
            // ARRANGE
            var container = new Container();
            var root = new GameRunner(container,
                new ConfigSettings(TestHelper.ConfigTestRootFile, TestHelper.GetPathResolver()));

            // ACT
            var modelBuilders = container.ResolveAll<IModelBuilder>().ToList();

            // ASSERT
            // NOTE change this value if you add/remove model builders
            Assert.AreEqual(7, modelBuilders.Count);
        }
    }
}