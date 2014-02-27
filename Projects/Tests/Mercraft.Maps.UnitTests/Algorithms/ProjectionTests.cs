using System;
using Mercraft.Core.Algorithms;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using NUnit.Framework;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Algorithms
{
    [TestFixture]
    public class ProjectionTests
    {
        private const double Percision = 0.000001;

        /// <summary>
        /// Berlin, Mitte (Eichendorffstraße)
        /// </summary>
        private GeoCoordinate _center = new GeoCoordinate(52.529814, 13.388015);

        /// <summary>
        /// Berlin, Tegel
        /// </summary>
        private GeoCoordinate _target = new GeoCoordinate(52.582922, 13.282957);

        [Test]
        public void CanConvertToMap()
        {
            var mapCoordinate = GeoProjection.ToMapCoordinate(_center, _target);

            Assert.AreEqual(-7114, Math.Truncate(mapCoordinate.x));
            Assert.AreEqual(5902, Math.Truncate(mapCoordinate.y));

            Assert.AreEqual(9244, Math.Truncate(Distance(new Vector2(0, 0), mapCoordinate)));
        }

        [Test]
        public void CanConvertToGeo()
        {
            // Arrange
            var mapCoordinate = GeoProjection.ToMapCoordinate(_center, _target);

            // Act
            var geoCoordinate = GeoProjection.ToGeoCoordinate(_center, mapCoordinate);

            Assert.True(Math.Abs(52.582922 - geoCoordinate.Latitude) < Percision);
            Assert.True(Math.Abs(13.282957 - geoCoordinate.Longitude) < Percision);
        }

        private static double Distance(Vector2 p1, Vector2 p2)
        {
            var diffX = p1.x - p2.x;
            var diffY = p1.y - p2.y;

            return Math.Sqrt(diffX * diffX + diffY * diffY);
        }
    }
}
