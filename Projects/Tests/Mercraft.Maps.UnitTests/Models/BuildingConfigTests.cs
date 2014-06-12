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
            // ARRANGE
            var settings = new ConfigSettings(TestHelper.ConfigTestRootFile, TestHelper.GetPathResolver());

            // ACT
            var styleProvider = new BuildingStyleProvider(settings.GetSection("buildings/styles"));

            // ASSERT
            AssertStyles(styleProvider);
        }

        [Test]
        public void CanReadTextureConfig()
        {
            // ARRANGE
            var settings = new ConfigSettings(TestHelper.ConfigTestRootFile, TestHelper.GetPathResolver());

            // ACT
            var textureProvider = new TexturePackProvider(settings.GetSection("buildings/textures"));

            // ASSERT
            AssertTextures(textureProvider);
        }

        [Test]
        public void CanReadConfigsFromContainer()
        {
            // ARRANGE
            var container = new Container();
            var root = new GameRunner(container, new ConfigSettings(TestHelper.ConfigTestRootFile, TestHelper.GetPathResolver()));

            var styleProvider = container.Resolve<BuildingStyleProvider>();
            AssertStyles(styleProvider);

            // ACT
            var textureProvider = container.Resolve<TexturePackProvider>();

            // ASSERT
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
