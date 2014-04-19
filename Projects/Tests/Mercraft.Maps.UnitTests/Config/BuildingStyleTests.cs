using Mercraft.Core;
using Mercraft.Explorer;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.UnitTests.Zones.Stubs;
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

        [Test]
        public void CanReadTexture()
        {
            var provider = new TexturePackProvider(TestHelper.BuildingStylesConfig);

            Assert.IsNotNull(provider.Get("berlin"));
            Assert.IsNotNull(provider.Get("minsk"));
        }
    }
}
