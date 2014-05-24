using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mercraft.Core;
using Mercraft.Core.MapCss;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Utilities;
using Mercraft.Explorer;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using NUnit.Framework;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.MapCss
{
    [TestFixture]
    class RuleEvalTests
    {
        [Test]
        public void CanUseCanvas()
        {
            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile);
            var stylesheet = provider.Get();
            var canvas = new Canvas();

            var rule = stylesheet.GetRule(canvas);
            var material = rule.Evaluate<string>("material");

            Assert.AreEqual("Terrain", material);
        }

        [Test]
        public void CanMergeDeclarations()
        {
            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile);
            var stylesheet = provider.Get();

            var area = new Area()
            {
                Id = 1,
                Points = new GeoCoordinate[]
                {
                    new GeoCoordinate(52.5212186,13.4096926),
			        new GeoCoordinate(52.5210184,13.4097473),
			        new GeoCoordinate(52.5209891,13.4097538),
			        new GeoCoordinate(52.5209766,13.4098037),
                },
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building","residential"),
                    new KeyValuePair<string, string>("building:shape","sphere"),
                    new KeyValuePair<string, string>("min_height","100"),
                    new KeyValuePair<string, string>("building:levels","5"),
                }
            };

            var rule = stylesheet.GetRule(area);

            Assert.IsTrue(rule.IsApplicable, "Unable to get declarations!");

            Assert.AreEqual("sphere", rule.Evaluate<string>("builder"), "Unable to merge declarations!");
            Assert.AreEqual(100, rule.Evaluate<float>("min_height"), "Unable to eval min_height from tag!");
            Assert.AreEqual(new Color32(250, 128, 114, 255), rule.GetFillColor(), "Unable to merge declarations!");
            Assert.AreEqual("solid", rule.Evaluate<string>("behaviour"), "First rule isn't applied!");
            Assert.AreEqual("Concrete_Patterned", rule.Evaluate<string>("material"), "First rule isn't applied!");
            Assert.AreEqual(15, rule.Evaluate<float>("height"), "Unable to eval height from building:levels!");
        }


        [Test]
        public void CanProcessSequence()
        {
            var testPoints = new GeoCoordinate[]
            {
                new GeoCoordinate(0, 0),
                new GeoCoordinate(0, 0),
                new GeoCoordinate(0, 0),
            };
            var area1 = new Area()
            {
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building", "tower"),
                    new KeyValuePair<string, string>("building:material", "metal"),
                    new KeyValuePair<string, string>("building:part", "yes"),
                    new KeyValuePair<string, string>("height", "237"),
                    new KeyValuePair<string, string>("min_height", "205"),
                },
                Points = testPoints
            };
            var area2 = new Area()
            {
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building", "roof"),
                    new KeyValuePair<string, string>("building:part", "yes"),
                    new KeyValuePair<string, string>("level", "1"),
                },
                Points = testPoints
            };

            var container = new Container();
            var componentRoot = new GameRunner(container, new ConfigSettings(TestHelper.ConfigTestRootFile));
            var stylesheet = container.Resolve<IStylesheetProvider>().Get();

            var rule1 = stylesheet.GetRule(area1);
            var rule2 = stylesheet.GetRule(area2);
            Assert.AreEqual(237, rule1.GetHeight());
            Assert.AreEqual(12f, rule2.GetHeight());
        }

        [Test]
        public void CanUseSimpleEvaluate()
        {
            var model = new Area()
            {
                Id = 1,
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building:levels", "5")
                }
            };

            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile);
            var stylesheet = provider.Get();

            var evalDeclaration = stylesheet.Styles[3].Declarations[0];

            var evalResult = evalDeclaration.Evaluator.Walk<float>(model);

            Assert.AreEqual(15, evalResult);

        }

        [Test]
        public void CanGetMissing()
        {
            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile);
            var stylesheet = provider.Get();

            var area = new Area()
            {
                Id = 1,
                Points = new GeoCoordinate[0],
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building","residential"),
                }
            };
            var rule = stylesheet.GetRule(area);

            Assert.AreEqual(0, rule.GetLevels());

        }

        [Test]
        public void CanGetColorByRGB()
        {
            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile);
            var stylesheet = provider.Get();

            var buildingWithColorCode = new Area()
            {
                Id = 1,
                Points = new GeoCoordinate[0],
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building","commercial"),
                }
            };
            var rule = stylesheet.GetRule(buildingWithColorCode);
            Assert.AreEqual(ColorUtility.FromName("red"), rule.GetFillColor());
        }

        [Test]
        public void CanGetColorByName()
        {
            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile);
            var stylesheet = provider.Get();

            var buildingWithColorName = new Area()
            {
                Id = 1,
                Points = new GeoCoordinate[0],
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building","yes"),
                }
            };
            var rule = stylesheet.GetRule(buildingWithColorName);
            Assert.AreEqual(ColorUtility.FromName("salmon"), rule.GetFillColor());         
        }

        [Test]
        public void CanApplyColorByRGB()
        {
            var provider = new StylesheetProvider(TestHelper.DefaultMapcssFile);
            var stylesheet = provider.Get();

            var buildingWithColorCode = new Area()
            {
                Id = 1,
                Points = new GeoCoordinate[0],
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building","commercial"),
                    new KeyValuePair<string, string>("building:color","#cfc6b5"),
                }
            };
            var rule = stylesheet.GetRule(buildingWithColorCode);
            Assert.AreEqual(ColorUtility.FromUnknown("#cfc6b5"), rule.GetFillColor());
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
    }
}
