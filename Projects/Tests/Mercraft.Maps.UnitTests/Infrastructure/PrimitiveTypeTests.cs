using ActionStreetMap.Infrastructure.Primitives;
using ActionStreetMap.Maps.UnitTests.Infrastructure.Stubs;
using NUnit.Framework;

namespace ActionStreetMap.Maps.UnitTests.Infrastructure
{
    [TestFixture]
    public class PrimitiveTypeTests
    {
        [Test]
        public void CanUseTuple()
        {
            // ARRANGE & ACT
            var tuple1 = new Tuple<string, int>("key", 7);
            var tuple2 = new Tuple<string, int>("key", 7);
            
            // ASSERT
            Assert.AreEqual("key", tuple1.Item1);
            Assert.AreEqual(7, tuple1.Item2);
            Assert.AreEqual(tuple1, tuple2);
        }
    }
}