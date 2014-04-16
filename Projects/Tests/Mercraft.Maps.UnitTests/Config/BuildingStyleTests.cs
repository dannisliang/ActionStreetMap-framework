using Mercraft.Models.Buildings.Config;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Config
{
    [TestFixture]
    public class BuildingStyleTests
    {
        [Test]
        public void CanReadStyle()
        {
            var provider = new BuildingStyleProvider(TestHelper.BuildingStylesConfig);

            Assert.IsNotNull(provider.Get("berlin", "residential"));
            Assert.IsNotNull(provider.Get("berlin", "commercial"));
            Assert.IsNotNull(provider.Get("minsk", "residential"));
            Assert.IsNotNull(provider.Get("minsk", "commercial"));
        }
    }
}
