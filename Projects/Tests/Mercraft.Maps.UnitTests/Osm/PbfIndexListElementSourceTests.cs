using System.Linq;
using Mercraft.Core;
using Mercraft.Infrastructure.Diagnostic;
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
                TestHelper.GetFileSystemService(), new DefaultTrace());

            // ACT
            var elements = elementSource.Get(
                BoundingBox.CreateBoundingBox(TestHelper.BerlinInvalidenStr, 100));

            // ASSERT
            Assert.AreEqual(elements.Count(), 577);
        }
    }
}