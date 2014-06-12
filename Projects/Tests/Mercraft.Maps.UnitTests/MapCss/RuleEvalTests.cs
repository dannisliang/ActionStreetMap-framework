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
    internal class RuleEvalTests
    {
        [Test]
        public void CanUseCanvas()
        {
            // ARRANGE
            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile, TestHelper.GetPathResolver());
            var stylesheet = provider.Get();
            var canvas = new Canvas();

            // ACT
            var rule = stylesheet.GetRule(canvas);
            var material = rule.Evaluate<string>("material");

            // ASSERT
            Assert.AreEqual("Terrain", material);
        }

        [Test]
        public void CanMergeDeclarations()
        {
            // ARRANGE
            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile, TestHelper.GetPathResolver());
            var stylesheet = provider.Get();

            var area = new Area
            {
                Id = 1,
                Points = new[]
                {
                    new GeoCoordinate(52.5212186, 13.4096926),
                    new GeoCoordinate(52.5210184, 13.4097473),
                    new GeoCoordinate(52.5209891, 13.4097538),
                    new GeoCoordinate(52.5209766, 13.4098037)
                },
                Tags = new Collection<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("building", "residential"),
                    new KeyValuePair<string, string>("building:shape", "sphere"),
                    new KeyValuePair<string, string>("min_height", "100"),
                    new KeyValuePair<string, string>("building:levels", "5"),
                }
            };

            // ACT
            var rule = stylesheet.GetRule(area);


            // ASSERT
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
            // ARRANGE
            var testPoints = new[]
            {
                new GeoCoordinate(0, 0),
                new GeoCoordinate(0, 0),
                new GeoCoordinate(0, 0)
            };
            var area1 = new Area
            {
                Tags = new Collection<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("building", "tower"),
                    new KeyValuePair<string, string>("building:material", "metal"),
                    new KeyValuePair<string, string>("building:part", "yes"),
                    new KeyValuePair<string, string>("height", "237"),
                    new KeyValuePair<string, string>("min_height", "205"),
                },
                Points = testPoints
            };
            var area2 = new Area
            {
                Tags = new Collection<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("building", "roof"),
                    new KeyValuePair<string, string>("building:part", "yes"),
                    new KeyValuePair<string, string>("level", "1"),
                },
                Points = testPoints
            };

            var container = new Container();
            var pathResolver = TestHelper.GetPathResolver();
            container.RegisterInstance(typeof (IPathResolver), pathResolver);
            var componentRoot = new GameRunner(container,
                new ConfigSettings(TestHelper.ConfigTestRootFile, pathResolver));
            var stylesheet = container.Resolve<IStylesheetProvider>().Get();

            // ACT
            var rule1 = stylesheet.GetRule(area1);
            var rule2 = stylesheet.GetRule(area2);

            // ASSERT
            Assert.AreEqual(237, rule1.GetHeight());
            Assert.AreEqual(12f, rule2.GetHeight());
        }

        [Test]
        public void CanUseSimpleEvaluate()
        {
            // ARRANGE
            var model = new Area
            {
                Id = 1,
                Tags = new Collection<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("building:levels", "5")
                }
            };

            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile, TestHelper.GetPathResolver());
            var stylesheet = provider.Get();
            var evalDeclaration = stylesheet.Styles[3].Declarations[0];

            // ACT
            var evalResult = evalDeclaration.Evaluator.Walk<float>(model);

            // ASSERT
            Assert.AreEqual(15, evalResult);
        }

        [Test]
        public void CanGetMissing()
        {
            // ARRANGE
            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile, TestHelper.GetPathResolver());
            var stylesheet = provider.Get();

            var area = new Area
            {
                Id = 1,
                Points = new GeoCoordinate[0],
                Tags = new Collection<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("building", "residential"),
                }
            };

            // ACT
            var rule = stylesheet.GetRule(area);

            // ASSERT
            Assert.AreEqual(0, rule.GetLevels());
        }

        [Test]
        public void CanUseAndSelectors()
        {
            // ARRANGE
            var stylesheet = MapCssHelper.GetStylesheet("way[waterway][name],way[waterway] { z-index: 0.1}\n");

            // ACT
            var way1 = MapCssHelper.GetWay(new KeyValuePair<string, string>("waterway", "river"),
                new KeyValuePair<string, string>("name", "spree"));
            var way2 = MapCssHelper.GetWay(new KeyValuePair<string, string>("name", "some name"));

            // ASSERT
            Assert.IsTrue(stylesheet.GetRule(way1).IsApplicable);
            Assert.IsFalse(stylesheet.GetRule(way2).IsApplicable);
        }

        [Test]
        public void CanGetColorByRGB()
        {
            // ARRANGE
            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile, TestHelper.GetPathResolver());
            var stylesheet = provider.Get();

            var buildingWithColorCode = new Area
            {
                Id = 1,
                Points = new GeoCoordinate[0],
                Tags = new Collection<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("building", "commercial"),
                }
            };

            // ACT
            var rule = stylesheet.GetRule(buildingWithColorCode);

            // ASSERT
            Assert.AreEqual(ColorUtility.FromName("red"),
                GetOriginalColorTypeObject(rule.GetFillColor()));
        }

        [Test]
        public void CanGetColorByName()
        {
            // ARRANGE
            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile, TestHelper.GetPathResolver());
            var stylesheet = provider.Get();

            var buildingWithColorName = new Area
            {
                Id = 1,
                Points = new GeoCoordinate[0],
                Tags = new Collection<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("building", "yes"),
                }
            };

            // ACT
            var rule = stylesheet.GetRule(buildingWithColorName);

            // ASSERT
            Assert.AreEqual(ColorUtility.FromName("salmon"),
                GetOriginalColorTypeObject(rule.GetFillColor()));
        }

        [Test]
        public void CanApplyColorByRGB()
        {
            // ARRANGE
            var provider = new StylesheetProvider(TestHelper.DefaultMapcssFile, TestHelper.GetPathResolver());
            var stylesheet = provider.Get();

            var buildingWithColorCode = new Area
            {
                Id = 1,
                Points = new GeoCoordinate[0],
                Tags = new Collection<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("building", "commercial"),
                    new KeyValuePair<string, string>("building:color", "#cfc6b5"),
                }
            };

            // ACT
            var rule = stylesheet.GetRule(buildingWithColorCode);

            // ASSERT
            Assert.AreEqual(ColorUtility.FromUnknown("#cfc6b5"),
                GetOriginalColorTypeObject(rule.GetFillColor()));
        }

        private Mercraft.Core.Unity.Color32 GetOriginalColorTypeObject(Color32 color)
        {
            return new Mercraft.Core.Unity.Color32(color.r, color.g, color.b, color.a);
        }
    }
}