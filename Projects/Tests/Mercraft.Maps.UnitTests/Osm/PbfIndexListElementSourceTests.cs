using System.Linq;
using Mercraft.Core;
using Mercraft.Maps.Osm.Data;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Osm
{
    [TestFixture]
    public class PbfIndexListElementSourceTests
    {
        [Test]
        public void CanReadIndex()
        {
            // ARRANGE
            var elementSource = new PbfIndexListElementSource(TestHelper.TestBigPbfIndexListPath,
                new TestPathResolver());

            // ACT
            var elements = elementSource.Get(
                BoundingBox.CreateBoundingBox(TestHelper.BerlinInvalidenStr, 100));

            // ASSERT
            Assert.AreEqual(elements.Count(), 577);
        }
    }
}