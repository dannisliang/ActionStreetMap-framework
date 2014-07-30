using System.Linq;
using System.Xml.Linq;
using Mercraft.Infrastructure.Config;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Infrastructure
{
    [TestFixture]
    public class ConfigMergerTests
    {
        [Test]
        public void CanMergeUsingKey()
        {
            // ARRANGE
            var element1 = XElement.Parse("<g><c merge=\"name\" name=\"key1\"><content1 /></c></g>");
            var element2 = XElement.Parse("<g><c merge=\"name\" name=\"key1\"><content3/></c><c merge=\"name\" name=\"key2\"><content2 /></c></g>");

            // ACT
            var result = ConfigMerger.Merge(element1, element2);
            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Elements().Count());
        }

        [Test]
        [ExpectedException]
        [Description("Current merge algorithm doesn't support non sequential elements with the same merge key")]
        public void CannotMergeNonSequentialy()
        {
            // ARRANGE
            var element1 = XElement.Parse("<g><c merge=\"name\" name=\"key1\"><content1 /></c></g>");
            var element2 = XElement.Parse("<g><c merge=\"name\" name=\"key2\"><content2 /></c><c merge=\"name\" name=\"key1\"><content3/></c></g>");

            // ACT
            var result = ConfigMerger.Merge(element1, element2);
            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Elements().Count());
        }
    }
}
