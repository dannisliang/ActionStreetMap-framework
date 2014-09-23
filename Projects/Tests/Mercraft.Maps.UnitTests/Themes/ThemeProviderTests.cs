using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Xml.Linq;
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
            var theme = provider.Get();

            // ASSERT
            Assert.IsNotNull(theme);

            var style = theme.GetBuildingStyle(new Building()
            {
                Type = "residential"
            });

            Assert.IsNotNull(style);
            Assert.IsNotNull(style.Facade);

            Assert.AreEqual(9, style.Facade.Floors);
            
            Assert.AreEqual("Textures/Buildings/Soviet1", style.Facade.Textures[0]);
            Assert.AreEqual("Materials/Buildings/Building", style.Facade.Materials[0]);
            Assert.AreEqual(4, style.Facade.FrontUvMap.Length);
            Assert.AreEqual(4, style.Facade.BackUvMap.Length);
            Assert.AreEqual(4, style.Facade.SideUvMap.Length);
            Assert.IsNotNull(style.Facade.Builders);

            Assert.IsNotNull(style.Roof);
            Assert.AreEqual("Textures/Buildings/Soviet1", style.Roof.Textures[0]);
            Assert.AreEqual("Materials/Buildings/Building", style.Roof.Materials[0]);
            Assert.AreEqual(2, style.Roof.UnitSize);
            Assert.IsNotNull(style.Roof.Builders);
        }

        [Test]
        public void CanGetRoadStyle()
        {
            // ARRANGE
            var provider = GetThemeProvider();
            provider.Configure(GetTestThemeConfig());

            // ACT
            var theme = provider.Get();

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
            Assert.AreEqual("Textures/Roads/Road", style.Textures[0]);
            Assert.AreEqual("Materials/Roads/Road", style.Materials[0]);
            Assert.IsNotNull(style.UvMap);
            Assert.AreEqual(4, style.UvMap.Main.Length);
            Assert.AreEqual(3, style.UvMap.Turn.Length);
        }

        private ThemeProvider GetThemeProvider()
        {
            return new ThemeProvider(
                new TestPathResolver(), 
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
            return new ConfigSection(TestHelper.TestThemeFile);
        }
    }
}
