using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercraft.Core.MapCss;
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
            Assert.AreEqual(5, stylesheet.Rules[0].Declarations.Count);
            Assert.AreEqual("node", stylesheet.Rules[0].Selectors[0].Type);
            Assert.AreEqual("place", stylesheet.Rules[0].Selectors[0].Tag);
            Assert.AreEqual("town", stylesheet.Rules[0].Selectors[0].Value);
            Assert.AreEqual("=", stylesheet.Rules[0].Selectors[0].Operation);

            Assert.AreEqual("building", stylesheet.Rules[7].Selectors[0].Tag);
            Assert.AreEqual("OP_EXIST", stylesheet.Rules[7].Selectors[0].Operation);
            
            Assert.AreEqual(6, stylesheet.Rules[15].Selectors.Count);
            Assert.AreEqual(6, stylesheet.Rules[15].Declarations.Count);
        }

    }
}
