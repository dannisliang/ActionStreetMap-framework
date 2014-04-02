using System.Linq;
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
            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile);
            var stylesheet = provider.Get();
            Assert.IsNotNull(stylesheet);
        }

        [Test]
        public void CanParse()
        {
            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile);
            var stylesheet = provider.Get();

            Assert.AreEqual(9, stylesheet.Styles.Count);

            var testStyle1 = stylesheet.Styles[1];

            Assert.AreEqual(2, testStyle1.Selectors.Count);
            Assert.AreEqual(7, testStyle1.Declarations.Count);

            var testSelector1 = testStyle1.Selectors.First();
            Assert.AreEqual("man_made", testSelector1.Tag);
            Assert.AreEqual("tower", testSelector1.Value);
            Assert.AreEqual("=", testSelector1.Operation);

            var testSelector2 = testStyle1.Selectors.Last();

            Assert.AreEqual("building", testSelector2.Tag);
            Assert.AreEqual("OP_EXIST", testSelector2.Operation);

            var lastStyle = stylesheet.Styles[7];
            Assert.AreEqual(2, lastStyle.Selectors.Count);
            Assert.AreEqual(1, lastStyle.Declarations.Count);
        }

      
    }
}
