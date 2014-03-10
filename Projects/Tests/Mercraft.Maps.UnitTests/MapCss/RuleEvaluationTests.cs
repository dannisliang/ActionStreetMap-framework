using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.MapCss;
using Mercraft.Core.Scene.Models;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.MapCss
{
    [TestFixture]
    public class RuleEvaluationTests
    {
        [Test]
        public void CanEvaluateHeight()
        {
            var provider = new StylesheetProvider(TestHelper.EvalMapcssFile);
            var stylesheet = provider.Get();

            var area = new Area()
            {
                Id = "1",
                Points = new Collection<GeoCoordinate>(),
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building","residental"),
                }
            };

            var applicableRules = stylesheet.Rules.Where(r => r.IsApplicable(area)).ToList();

            var height = applicableRules[0].Evaluate<int>(area, "height");

            Assert.AreEqual(10, height);
        }

        [Test]
        public void CanEvaluateHeightWithEval()
        {
            var provider = new StylesheetProvider(TestHelper.EvalMapcssFile);
            var stylesheet = provider.Get();

            var area = new Area()
            {
                Id = "1",
                Points = new Collection<GeoCoordinate>(),
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building","residental"),
                    new KeyValuePair<string, string>("building:levels","4"),
                }
            };

            var applicableRules = stylesheet.Rules.Where(r => r.IsApplicable(area)).ToList();

            var height = applicableRules[0].Evaluate<int>(area, "height");

            Assert.AreEqual(10, height);
        }
    }
}
