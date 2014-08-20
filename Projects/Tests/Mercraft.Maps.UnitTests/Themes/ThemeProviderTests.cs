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
                Type = "residental"
            });

            Assert.AreEqual(9, style.Floors);
            
            Assert.IsNotNull(style.Facade);
            Assert.AreEqual("Textures/Buildings/Soviet1", style.Facade.Texture);
            Assert.AreEqual("Materials/Building", style.Facade.Material);
            Assert.AreEqual(4, style.Facade.FrontUvMap.Length);
            Assert.AreEqual(4, style.Facade.BackUvMap.Length);
            Assert.AreEqual(4, style.Facade.SideUvMap.Length);
            Assert.IsNotNull(style.Facade.Builder);

            Assert.IsNotNull(style.Roof);
            Assert.AreEqual("Textures/Buildings/Soviet1", style.Roof.Texture);
            Assert.AreEqual("Materials/Building", style.Roof.Material);
            Assert.AreEqual(4, style.Roof.UvMap.Length);
            Assert.IsNotNull(style.Roof.Builder);
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
            Assert.AreEqual("Textures2", style.TextureKey);
            Assert.AreEqual("Materials2", style.MaterialKey);
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
