
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
        public void CanLoadTheme()
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
            Assert.IsNotNull(style.TextureMap);
            Assert.AreEqual(4, style.TextureMap.FrontUv.Length);
            Assert.AreEqual(4, style.TextureMap.BackUv.Length);
            Assert.AreEqual(4, style.TextureMap.SideUv.Length);
            Assert.AreEqual(4, style.TextureMap.RoofUv.Length);
        }

        private IConfigSection GetTestThemeConfig()
        {
            var appDocument = XDocument.Load(TestHelper.TestThemeFile);
            return (new ConfigSection(new ConfigElement(appDocument.Root))).GetSection("themes");
        }
    }
}
