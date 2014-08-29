using System.Linq;
using Mercraft.Core.Scene;
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

            // it should fill container
            var root = TestHelper.GetGameRunner(container);

            // ACT
            var modelBuilders = container.ResolveAll<IModelBuilder>().ToList();

            // ASSERT
            // NOTE change this value if you add/remove model builders
            Assert.AreEqual(6, modelBuilders.Count);
        }
    }
}