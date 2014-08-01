using System.Collections.Generic;
using System.ComponentModel;
using Mercraft.Core.World.Buildings;
using Mercraft.Core.World.Roads;
using Mercraft.Explorer.Themes;
using Mercraft.Infrastructure.Config;
using Mercraft.Models.Buildings.Facades;
using Mercraft.Models.Buildings.Roofs;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Themes
{
    [TestFixture]
    public class ThemeProviderTests
    {
        [Test]
        public void CanGetBuildingStyle()
        {
            // ARRANGE
            var provider = GetThemeProvider();
            provider.Configure(GetTestThemeConfig());

            // ACT
            var theme = provider.Get("default");

            // ASSERT
            Assert.IsNotNull(theme);

            var style = theme.GetBuildingStyle( new Building()
            {
                Type = "residental1"
            });

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
        public void CanGetRoadStyle()
        {
            // ARRANGE
            var provider = GetThemeProvider();
            provider.Configure(GetTestThemeConfig());

            // ACT
            var theme = provider.Get("default");

            // ASSERT
            Assert.IsNotNull(theme);

            var style = theme.GetRoadStyle(new Road()
            {
                Elements = new List<RoadElement>()
                {
                    new RoadElement()
                    {
                        Type = "residental"
                    }
                }
            });
            Assert.AreEqual("Textures2", style.Texture);
            Assert.AreEqual("Materials2", style.Material);
            Assert.IsNotNull(style.UvMap);
            Assert.AreEqual(4, style.UvMap.Main.Length);
            Assert.AreEqual(3, style.UvMap.Turn.Length);
        }

        private ThemeProvider GetThemeProvider()
        {
            return new ThemeProvider(
                new List<IFacadeBuilder>()
                {
                   new FlatFacadeBuilder()
                }, 
                new List<IRoofBuilder>()
                {
                    new FlatRoofBuilder()
                });
        }

        private IConfigSection GetTestThemeConfig()
        {
            var settings = new ConfigSettings(TestHelper.TestThemeFile, TestHelper.GetPathResolver());
            return settings.GetRoot();
        }
    }
}
