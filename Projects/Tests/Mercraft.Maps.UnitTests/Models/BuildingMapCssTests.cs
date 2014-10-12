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
            // ARRANGE
            var provider = new StylesheetProvider(TestHelper.DefaultMapcssFile, TestHelper.GetFileSystemService());
            var stylesheet = provider.Get();

            var building = new Area()
            {
                Id = 1,
                Points = new List<GeoCoordinate>(),
                Tags = new Dictionary<string, string>()
                {
                    {"building","residential"},
                }
            };

            // ACT
            var rule = stylesheet.GetModelRule(building);
            var style = rule.GetBuildingType();

            // ASSERT
            Assert.AreEqual("residential", style);
        }
    }
}
