
using System.Collections.Generic;
using System.IO;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Formats.Pbf;
using Mercraft.Maps.Osm.Formats.Xml;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Osm
{
    [TestFixture]
    class FormatTests
    {
        [Test]
        public void CanPullPbfStream()
        {
            using (Stream stream = new FileStream(TestHelper.TestPbfFilePath, FileMode.Open))
            {
                var source = new PbfOsmStreamSource(stream);
                source.Initialize();
                
                var elements = new List<Element>();
                while (source.MoveNext())
                {
                    Element element = source.Current();
                    elements.Add(element);
                }

                Assert.AreEqual(137500, elements.Count);
            }
        }

        [Test]
        public void CanPullXmlStream()
        {
            using (Stream stream = new FileStream(TestHelper.TestXmlFilePath, FileMode.Open))
            {
                var source = new XmlOsmStreamSource(stream);
                source.Initialize();
                
                var elements = new List<Element>();
                while (source.MoveNext())
                {
                    Element element = source.Current();
                    elements.Add(element);
                }

                Assert.AreEqual(428, elements.Count);
            }
        }
    }
}
