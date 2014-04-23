using Mercraft.Explorer;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Buildings.Config;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Models
{
    [TestFixture]
    public class BuildingConfigTests
    {
        [Test]
        public void CanReadStyleConfig()
        {
            var settings = new ConfigSettings(TestHelper.ConfigRootFile);
            var styleProvider = new BuildingStyleProvider(settings.GetSection("buildings/styles"));
            AssertStyles(styleProvider);
        }

        [Test]
        public void CanReadTextureConfig()
        {
            var settings = new ConfigSettings(TestHelper.ConfigRootFile);
            var textureProvider = new TexturePackProvider(settings.GetSection("buildings/textures"));
            AssertTextures(textureProvider);
        }

        [Test]
        public void CanReadConfigsFromContainer()
        {
            var container = new Container();
            var root = new GameRunner(container, new ConfigSettings(TestHelper.ConfigRootFile));

            var styleProvider = container.Resolve<BuildingStyleProvider>();
            AssertStyles(styleProvider);

            var textureProvider = container.Resolve<TexturePackProvider>();
            AssertTextures(textureProvider);
        }


        #region Helper methods

        private void AssertStyles(BuildingStyleProvider styleProvider)
        {
            Assert.IsNotNull(styleProvider.Get("berlin", "residential"));
            Assert.IsNotNull(styleProvider.Get("berlin", "commercial"));
            Assert.IsNotNull(styleProvider.Get("minsk", "residential"));
            Assert.IsNotNull(styleProvider.Get("minsk", "commercial"));
        }

        private void AssertTextures(TexturePackProvider textureProvider)
        {
            Assert.IsNotNull(textureProvider.Get("asian"));
            Assert.IsNotNull(textureProvider.Get("europe"));
            Assert.IsNotNull(textureProvider.Get("georgian"));
        }

        #endregion
    }
}
