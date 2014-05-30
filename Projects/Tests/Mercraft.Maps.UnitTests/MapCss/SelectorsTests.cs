using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Mercraft.Core;
using Mercraft.Core.MapCss;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.MapCss
{
    [TestFixture]
    public class SelectorsTests
    {
        [Test]
        public void CanUseEqual()
        {
            var stylesheet = GetStylesheet("area[landuse=forest] { z-index: 0.1}\n");
            var area1 = GetArea(new KeyValuePair<string, string>("landuse", "forest"));
            var area2 = GetArea(new KeyValuePair<string, string>("landuse", "grass"));

            Assert.IsTrue(stylesheet.GetRule(area1).IsApplicable);
            Assert.IsFalse(stylesheet.GetRule(area2).IsApplicable);
        }

        [Test]
        public void CanUseNotEqual()
        {
            var stylesheet = GetStylesheet("area[landuse!=forest] { z-index: 0.1}\n");
            var area1 = GetArea(new KeyValuePair<string, string>("landuse", "forest"));
            var area2 = GetArea(new KeyValuePair<string, string>("landuse", "grass"));

            Assert.IsTrue(stylesheet.GetRule(area1).IsApplicable);
            Assert.IsFalse(stylesheet.GetRule(area2).IsApplicable);
        }

        [Test]
        public void CanUseLess()
        {
            var stylesheet = GetStylesheet("area[level<0] { z-index: 0.1}\n");
            var area1 = GetArea(new KeyValuePair<string, string>("level", "-1"));
            var area2 = GetArea(new KeyValuePair<string, string>("level", "1"));

            Assert.IsTrue(stylesheet.GetRule(area1).IsApplicable);
            Assert.IsFalse(stylesheet.GetRule(area2).IsApplicable);
        }

        [Test]
        public void CanUseGreater()
        {
            var stylesheet = GetStylesheet("area[level>0] { z-index: 0.1}\n");
            var area1 = GetArea(new KeyValuePair<string, string>("level", "1"));
            var area2 = GetArea(new KeyValuePair<string, string>("level", "0"));

            Assert.IsTrue(stylesheet.GetRule(area1).IsApplicable);
            Assert.IsFalse(stylesheet.GetRule(area2).IsApplicable);
        }

        [Test]
        public void CanUseClosed()
        {
            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile);
            var stylesheet = provider.Get();

            var closedWay = new Way()
            {
                Points = new[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 0),
                },
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("barrier", "yes")
                }
            };

            Assert.IsTrue(stylesheet.GetRule(closedWay).IsApplicable);


            var openWay = new Way()
            {
                Points = new[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 1),
                },
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("barrier", "yes")
                }
            };

            Assert.IsFalse(stylesheet.GetRule(openWay).IsApplicable);

        }

        private Area GetArea(params KeyValuePair<string, string>[] tags)
        {
            return new Area()
            {
                Points = new[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 0),
                },
                Tags = tags
            };
        }

        private Stylesheet GetStylesheet(string content)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;
            var provider = new StylesheetProvider(stream);
            return provider.Get();
        }
    }
}