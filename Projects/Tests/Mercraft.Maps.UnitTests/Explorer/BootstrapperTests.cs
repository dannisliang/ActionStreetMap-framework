using System.Linq;
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
            var container = new Container();
            var root = new GameRunner(container, new ConfigSettings(TestHelper.ConfigTestRootFile));

            var modelBuilders = container.ResolveAll<IModelBuilder>().ToList();

            // NOTE change this value if you add/remove model builders
            Assert.AreEqual(5, modelBuilders.Count);
        }
    }
}