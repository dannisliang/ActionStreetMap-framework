using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Mercraft.Maps.Osm.Helpers;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Osm
{
    [TestFixture]
    public class LocationInfoTests
    {
        [Test]
        public void CanExtractLocationInfo()
        {
            var tags = new Collection<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("addr:housenumber", "26"),
                new KeyValuePair<string, string>("addr:postcode", "220088"),
                new KeyValuePair<string, string>("addr:street", "Zacharova"),
            };

            var locationInfo = LocationInfoExtractor.Extract(tags);

            Assert.AreEqual("26", locationInfo.Name);
            Assert.AreEqual("Zacharova", locationInfo.Street);
            Assert.AreEqual("220088", locationInfo.Code);
        }
    }
}
