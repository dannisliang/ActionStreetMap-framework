using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Mercraft.Core;
using Mercraft.Core.MapCss;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.MapCss
{
    [TestFixture]
    public class StylesheetTests
    {
        [Test]
        public void CanCreate()
        {
            var provider = new StylesheetProvider(TestHelper.TestMapcssFile);
            var stylesheet = provider.Get();
            Assert.IsNotNull(stylesheet);
        }

        [Test]
        public void CanParse()
        {
            var provider = new StylesheetProvider(TestHelper.TestMapcssFile);
            var stylesheet = provider.Get();
            
            Assert.AreEqual(20, stylesheet.Rules.Count);

            Assert.AreEqual(1, stylesheet.Rules[1].Selectors.Count);
            Assert.AreEqual(2, stylesheet.Rules[1].Declarations.Count);
            
            Assert.AreEqual("place", stylesheet.Rules[1].Selectors[0].Tag);
            Assert.AreEqual("town", stylesheet.Rules[1].Selectors[0].Value);
            Assert.AreEqual("=", stylesheet.Rules[1].Selectors[0].Operation);

            Assert.AreEqual("building", stylesheet.Rules[3].Selectors[0].Tag);
            Assert.AreEqual("OP_EXIST", stylesheet.Rules[3].Selectors[0].Operation);
            
            Assert.AreEqual(6, stylesheet.Rules[16].Selectors.Count);
            Assert.AreEqual(6, stylesheet.Rules[16].Declarations.Count);
        }

        [Test]
        public void CanParseOrSelectors()
        {
            var provider = new StylesheetProvider(TestHelper.OrMapcssFile);
            var stylesheet = provider.Get();

            var matchOne1 = new Area()
            {
                Id = 1,
                Points = new GeoCoordinate[0],
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building","residential"),
                }
            };

            var matchOne2 = new Area()
            {
                Id = 1,
                Points = new GeoCoordinate[0],
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building","yes"),
                    new KeyValuePair<string, string>("whatever","123"),
                }
            };

            var matchOne3 = new Area()
            {
                Id = 1,
                Points = new GeoCoordinate[0],
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building","yes"),
                    new KeyValuePair<string, string>("whatever",""),
                }
            };

            var matchAll = new Area()
            {
                Id = 1,
                Points = new GeoCoordinate[0],
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building","residential"),
                    new KeyValuePair<string, string>("building","yes"),
                    new KeyValuePair<string, string>("whatever","1"),
                }
            };


            Assert.IsNotNull(stylesheet.GetRule(matchOne1));
            Assert.AreEqual("polygon", stylesheet.GetRule(matchOne1).Evaluate<string>(matchOne1, "build"));

            Assert.IsNotNull(stylesheet.GetRule(matchOne2));
            Assert.AreEqual("polygon", stylesheet.GetRule(matchOne2).Evaluate<string>(matchOne2, "build"));

            Assert.IsNotNull(stylesheet.GetRule(matchOne3));
            Assert.AreEqual("polygon", stylesheet.GetRule(matchOne3).Evaluate<string>(matchOne3, "build"));

            Assert.IsNotNull(stylesheet.GetRule(matchAll));
            Assert.AreEqual("unknown", stylesheet.GetRule(matchAll).Evaluate<string>(matchAll, "build"));

        }

        [Test]
        public void CanParseClosed()
        {
            var provider = new StylesheetProvider(TestHelper.TestMapcssFile);
            var stylesheet = provider.Get();

            var closedRule = stylesheet.Rules[19];
            Assert.AreEqual(2, closedRule.Selectors.Count);
            Assert.AreEqual("someclosed", closedRule.Selectors[0].Tag);
            Assert.AreEqual("OP_EXIST", closedRule.Selectors[0].Operation);
            Assert.AreEqual(true, closedRule.Selectors[1].IsClosed);
        }

    }
}
