using System.Linq;
using ActionStreetMap.Core;
using ActionStreetMap.Infrastructure.Diagnostic;
using ActionStreetMap.Maps.Osm.Data;
using NUnit.Framework;

namespace ActionStreetMap.Maps.UnitTests.Osm
{
    [TestFixture]
    public class PbfIndexListElementSourceTests
    {
        [Test]
        public void CanReadIndex()
        {
            // ARRANGE
            var elementSource = new PbfIndexListElementSource(TestHelper.TestBigPbfIndexListPath,
                TestHelper.GetFileSystemService(), new DefaultTrace());

            // ACT
            var elements = elementSource.Get(
                BoundingBox.CreateBoundingBox(TestHelper.BerlinInvalidenStr, 100));

            // ASSERT
            Assert.AreEqual(elements.Count(), 577);
        }
    }
}