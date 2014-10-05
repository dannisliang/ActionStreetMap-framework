using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Core.Algorithms
{
    [TestFixture]
    public class ProjectionTests
    {
        private const double Percision = 0.000001;

        /// <summary>
        ///     Berlin, Mitte (Eichendorffstraße)
        /// </summary>
        private readonly GeoCoordinate _center = new GeoCoordinate(52.529814, 13.388015);

        /// <summary>
        ///     Berlin, Tegel
        /// </summary>
        private readonly GeoCoordinate _target = new GeoCoordinate(52.582922, 13.282957);

        [Test]
        public void CanConvertToMap()
        {
            // ACT
            var mapCoordinate = GeoProjection.ToMapCoordinate(_center, _target);

            // ASSERT
            Assert.AreEqual(-7114, Math.Truncate(mapCoordinate.X));
            Assert.AreEqual(5902, Math.Truncate(mapCoordinate.Y));
            Assert.AreEqual(9244, Math.Truncate(Distance(new MapPoint(0, 0), mapCoordinate)));
        }

        [Test]
        public void CanConvertToGeo()
        {
            // ARRANGE
            var mapCoordinate = GeoProjection.ToMapCoordinate(_center, _target);

            // ACT
            var geoCoordinate = GeoProjection.ToGeoCoordinate(_center, mapCoordinate);

            // ASSERT
            Assert.True(Math.Abs(52.582922 - geoCoordinate.Latitude) < Percision);
            Assert.True(Math.Abs(13.282957 - geoCoordinate.Longitude) < Percision);
        }

        [Test(Description = "Tests correctness of traversal order sorting logic")]
        public void CanReverseVertices()
        {
            // ARRANGE
            var center = new GeoCoordinate(52.529814, 13.388015);
            var geoCoordinates = new List<GeoCoordinate>
            {
                new GeoCoordinate(52.5295083, 13.3889532),
                new GeoCoordinate(52.5291505, 13.3891865),
                new GeoCoordinate(52.5291244, 13.3891088),
                new GeoCoordinate(52.5291819, 13.389071),
                new GeoCoordinate(52.5291502, 13.3889361),
                new GeoCoordinate(52.529244, 13.3888741),
                new GeoCoordinate(52.5292772, 13.3890143),
                new GeoCoordinate(52.529354, 13.3889638),
                new GeoCoordinate(52.5293253, 13.3888356),
                new GeoCoordinate(52.5294599, 13.3887466),
            };

            // ACT & ASSERT
            var originalOrder = geoCoordinates.Select(g => GeoProjection.ToMapCoordinate(center, g)).ToArray();

            // direct order
            var points = new List<MapPoint>();
            PolygonHelper.GetVerticies2D(center, geoCoordinates, points);
            Assert.IsTrue(points.SequenceEqual(originalOrder));

            // reversed
            geoCoordinates.Reverse();
            points.Clear();
            PolygonHelper.GetVerticies2D(center, geoCoordinates, points);

            Assert.IsTrue(points.SequenceEqual(originalOrder));
        }

        private static double Distance(MapPoint p1, MapPoint p2)
        {
            var diffX = p1.X - p2.X;
            var diffY = p1.Y - p2.Y;

            return Math.Sqrt(diffX*diffX + diffY*diffY);
        }
    }
}