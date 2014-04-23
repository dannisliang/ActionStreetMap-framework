using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mercraft.Core;
using Mercraft.Core.MapCss;
using Mercraft.Core.Scene.Models;
using Mercraft.Explorer.Helpers;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Models
{
    [TestFixture]
    public class BuildingMapCssTests
    {
        [Test]
        public void CanGetBuildingStyle()
        {
            var provider = new StylesheetProvider(TestHelper.DefaultMapcssFile);
            var stylesheet = provider.Get();

            var building = new Area()
            {
                Id = 1,
                Points = new GeoCoordinate[0],
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building","residential"),
                }
            };

            var rule = stylesheet.GetRule(building);
            var style = rule.GetBuildingStyle();
            Assert.AreEqual("residential", style);
        }
    }
}
