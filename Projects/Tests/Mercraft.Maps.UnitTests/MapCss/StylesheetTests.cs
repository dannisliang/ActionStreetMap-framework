using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Mercraft.Core;
using Mercraft.Core.MapCss;
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
            
            Assert.AreEqual(18, stylesheet.Rules.Count);

            Assert.AreEqual(1, stylesheet.Rules[0].Selectors.Count);
            Assert.AreEqual(2, stylesheet.Rules[0].Declarations.Count);
            
            Assert.AreEqual("place", stylesheet.Rules[0].Selectors[0].Tag);
            Assert.AreEqual("town", stylesheet.Rules[0].Selectors[0].Value);
            Assert.AreEqual("=", stylesheet.Rules[0].Selectors[0].Operation);

            Assert.AreEqual("building", stylesheet.Rules[7].Selectors[0].Tag);
            Assert.AreEqual("OP_EXIST", stylesheet.Rules[7].Selectors[0].Operation);
            
            Assert.AreEqual(6, stylesheet.Rules[15].Selectors.Count);
            Assert.AreEqual(6, stylesheet.Rules[15].Declarations.Count);
        }

        [Test]
        public void CanFilterSeveralTags()
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

            Assert.AreEqual(1, applicableRules.Count);
            Assert.AreEqual(1, applicableRules[0].Selectors.Count);
        }

    }
}
