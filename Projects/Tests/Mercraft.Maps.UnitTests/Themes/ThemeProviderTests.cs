
using System.Xml.Linq;
using Mercraft.Explorer.Themes;
using Mercraft.Infrastructure.Config;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Themes
{
    [TestFixture]
    public class ThemeProviderTests
    {
        [Test]
        public void CanLoadBuildingTheme()
        {
            // ARRANGE
            var provider = new ThemeProvider();
            provider.Configure(GetTestThemeConfig());

            // ACT
            var theme = provider.Get("default");

            // ASSERT
            Assert.IsNotNull(theme);
            Assert.AreEqual(2, theme.BuildingTypeStyleMapping.Keys.Count);

            var styles = theme.BuildingTypeStyleMapping["residental1"];
            Assert.AreEqual(2, styles.Count);

            var style = styles[1];
            Assert.AreEqual(2, style.Floors);
            Assert.AreEqual("Texture2", style.Texture);
            Assert.AreEqual("Material2", style.Material);
            Assert.IsNotNull(style.UvMap);
            Assert.AreEqual(4, style.UvMap.Front.Length);
            Assert.AreEqual(4, style.UvMap.Back.Length);
            Assert.AreEqual(4, style.UvMap.Side.Length);
            Assert.AreEqual(4, style.UvMap.Roof.Length);
            Assert.IsNotNull(style.FacadeBuilder);
            Assert.IsNotNull(style.RoofBuilder);
        }

        [Test]
        public void CanLoadRoadTheme()
        {
            // ARRANGE
            var provider = new ThemeProvider();
            provider.Configure(GetTestThemeConfig());

            // ACT
            var theme = provider.Get("default");

            // ASSERT
            Assert.IsNotNull(theme);
            Assert.AreEqual(1, theme.RoadTypeStyleMapping.Keys.Count);

            var styles = theme.RoadTypeStyleMapping["residental"];
            Assert.AreEqual(2, styles.Count);
            var style = styles[1];
            Assert.AreEqual("Textures2", style.Texture);
            Assert.AreEqual("Materials2", style.Material);
            Assert.IsNotNull(style.UvMap);
            Assert.AreEqual(4, style.UvMap.Main.Length);
            Assert.AreEqual(3, style.UvMap.Turn.Length);
        }

        private IConfigSection GetTestThemeConfig()
        {
            var settings = new ConfigSettings(TestHelper.TestThemeFile, new TestPathResolver());
            return settings.GetRoot();
        }
    }
}
