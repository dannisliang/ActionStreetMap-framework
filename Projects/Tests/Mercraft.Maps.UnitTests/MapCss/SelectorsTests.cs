using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mercraft.Core;
using Mercraft.Core.MapCss;
using Mercraft.Core.Scene.Models;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.MapCss
{
    [TestFixture]
    public class SelectorsTests
    {
        [Test]
        public void CanUseExist()
        {
            // ARRANGE
            var stylesheet = MapCssHelper.GetStylesheet("area[landuse] { z-index: 0.1}\n");

            // ACT
            var area1 = MapCssHelper.GetArea(new KeyValuePair<string, string>("landuse", "forest"));
            var area2 = MapCssHelper.GetArea(new KeyValuePair<string, string>("building", "residential"));

            // ASSERT
            Assert.IsTrue(stylesheet.GetRule(area1).IsApplicable);
            Assert.IsFalse(stylesheet.GetRule(area2).IsApplicable);
        }

        [Test]
        public void CanUseNotExist()
        {
            // ARRANGE
            var stylesheet = MapCssHelper.GetStylesheet("area[!landuse] { z-index: 0.1}\n");

            // ACT
            var area1 = MapCssHelper.GetArea(new KeyValuePair<string, string>("landuse", "forest"));
            var area2 = MapCssHelper.GetArea(new KeyValuePair<string, string>("building", "residential"));

            // ASSERT
            Assert.IsFalse(stylesheet.GetRule(area1).IsApplicable);
            Assert.IsTrue(stylesheet.GetRule(area2).IsApplicable);
        }

        [Test]
        public void CanUseEqual()
        {
            // ARRANGE
            var stylesheet = MapCssHelper.GetStylesheet("area[landuse=forest] { z-index: 0.1}\n");

            // ACT
            var area1 = MapCssHelper.GetArea(new KeyValuePair<string, string>("landuse", "forest"));
            var area2 = MapCssHelper.GetArea(new KeyValuePair<string, string>("landuse", "grass"));

            // ASSERT
            Assert.IsTrue(stylesheet.GetRule(area1).IsApplicable);
            Assert.IsFalse(stylesheet.GetRule(area2).IsApplicable);
        }

        [Test]
        public void CanUseNotEqual()
        {
            // ARRANGE
            var stylesheet = MapCssHelper.GetStylesheet("area[landuse!=forest] { z-index: 0.1}\n");

            // ACT
            var area1 = MapCssHelper.GetArea(new KeyValuePair<string, string>("landuse", "forest"));
            var area2 = MapCssHelper.GetArea(new KeyValuePair<string, string>("landuse", "grass"));

            // ASSERT
            Assert.IsTrue(stylesheet.GetRule(area1).IsApplicable);
            Assert.IsFalse(stylesheet.GetRule(area2).IsApplicable);
        }

        [Test]
        public void CanUseLess()
        {
            // ARRANGE
            var stylesheet = MapCssHelper.GetStylesheet("area[level<0] { z-index: 0.1}\n");

            // ACT
            var area1 = MapCssHelper.GetArea(new KeyValuePair<string, string>("level", "-1"));
            var area2 = MapCssHelper.GetArea(new KeyValuePair<string, string>("level", "1"));

            // ASSERT
            Assert.IsTrue(stylesheet.GetRule(area1).IsApplicable);
            Assert.IsFalse(stylesheet.GetRule(area2).IsApplicable);
        }

        [Test]
        public void CanUseGreater()
        {
            // ARRANGE
            var stylesheet = MapCssHelper.GetStylesheet("area[level>0] { z-index: 0.1}\n");

            // ACT
            var area1 = MapCssHelper.GetArea(new KeyValuePair<string, string>("level", "1"));
            var area2 = MapCssHelper.GetArea(new KeyValuePair<string, string>("level", "0"));

            // ASSERT
            Assert.IsTrue(stylesheet.GetRule(area1).IsApplicable);
            Assert.IsFalse(stylesheet.GetRule(area2).IsApplicable);
        }

        [Test]
        public void CanUseClosed()
        {
            // ARRANGE
            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile, TestHelper.GetPathResolver());
            var stylesheet = provider.Get();

            var closedWay = new Way
            {
                Points = new[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 0)
                },
                Tags = new Collection<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("barrier", "yes")
                }
            };

            var openWay = new Way
            {
                Points = new[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 1)
                },
                Tags = new Collection<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("barrier", "yes")
                }
            };

            // ACT & ASSERT
            Assert.IsTrue(stylesheet.GetRule(closedWay).IsApplicable);
            Assert.IsFalse(stylesheet.GetRule(openWay).IsApplicable);
        }
    }
}