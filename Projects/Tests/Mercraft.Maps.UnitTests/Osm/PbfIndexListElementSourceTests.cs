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
            var elementSource = new PbfIndexListElementSource(TestHelper.TestBigPbfIndexListPath, 
                new TestPathResolver());

            var elements = elementSource.Get(
                BoundingBox.CreateBoundingBox(TestHelper.BerlinInvalidenStr, 100));

            Assert.AreEqual(elements.Count(), 577);
        }
    }
}
