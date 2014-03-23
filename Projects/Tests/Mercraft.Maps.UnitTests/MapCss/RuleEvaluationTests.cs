using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.MapCss;
using Mercraft.Core.Scene.Models;
using Mercraft.Explorer.Helpers;
using NUnit.Framework;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.MapCss
{
    [TestFixture]
    public class RuleEvaluationTests
    {
        [Test]
        public void CanGetAreaHeight()
        {
            var provider = new StylesheetProvider(TestHelper.EvalMapcssFile);
            var stylesheet = provider.Get();

            var area = new Area()
            {
                Id = 1,
                Points = new Collection<GeoCoordinate>(),
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building","residential"),
                }
            };

            var rule = stylesheet.GetRule(area);
            var height = rule.GetHeight(area);

            Assert.AreEqual(10, height);
        }

        [Test]
        public void CanGetAreaHeightWithEval()
        {
            var provider = new StylesheetProvider(TestHelper.EvalMapcssFile);
            var stylesheet = provider.Get();

            var area = new Area()
            {
                Id = 1,
                Points = new Collection<GeoCoordinate>(),
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building","residential"),
                    new KeyValuePair<string, string>("building:levels","4"),
                }
            };

            var rule = stylesheet.GetRule(area);
            var height = rule.GetHeight(area);

            Assert.AreEqual(8, height);
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

            var provider = new StylesheetProvider(TestHelper.EvalMapcssFile);
            var stylesheet = provider.Get();

            var evalDeclaration = stylesheet.Rules[1].Declarations[3];

            var evalResult = evalDeclaration.Evaluator.Walk<float>(model);

            Assert.AreEqual(10, evalResult);
            
        }

        [Test]
        public void CanUseCanvas()
        {
            var provider = new StylesheetProvider(TestHelper.MapcssFile);
            var stylesheet = provider.Get();
            var canvas = new Canvas();

            var rule = stylesheet.GetRule(canvas);
            var material = rule.Evaluate<string>(canvas, "material");

            Assert.AreEqual("Terrain", material);

        }

        [Test]
        public void CanUseAreaDeclarations()
        {
            var provider = new StylesheetProvider(TestHelper.EvalMapcssFile);
            var stylesheet = provider.Get();

            var area = new Area()
            {
                Id = 1,
                Points = new Collection<GeoCoordinate>(),
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building","residential"),
                }
            };
            var rule = stylesheet.GetRule(area);

            Assert.AreEqual("polygon", rule.Evaluate<string>(area, "builder"));

        }

        [Test]
        public void CanGetMissing()
        {
            var provider = new StylesheetProvider(TestHelper.EvalMapcssFile);
            var stylesheet = provider.Get();

            var area = new Area()
            {
                Id = 1,
                Points = new Collection<GeoCoordinate>(),
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building","residential"),
                }
            };
            var rule = stylesheet.GetRule(area);

            Assert.AreEqual(0, rule.GetLevels(area));

        }

        [Test]
        public void CanGetColor()
        {
            var provider = new StylesheetProvider(TestHelper.EvalMapcssFile);
            var stylesheet = provider.Get();

            var park = new Area()
            {
                Id = 1,
                Points = new Collection<GeoCoordinate>(),
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("leisure","park"),
                }
            };
            var rule = stylesheet.GetRule(park);

            Assert.AreEqual(new Color(34, 255, 17), rule.GetFillColor(park, Color.green));
        }
    }
}
