using System;
using Mercraft.Core;
using Mercraft.Core.Elevation;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Elevation
{
    [TestFixture]
    public class HeightMapTests
    {
        [TestCase(257, 100, 0, 0)]
        [TestCase(513, 500, 52, 13)]
        [TestCase(1025, 1000, 52, 13)]
        public void CanCreateHeightMapData(int resolution, float tileSize, double latitude, double longitude)
        {
            // ARRANGE 
            var center = new GeoCoordinate(latitude, longitude);
            var bbox = BoundingBox.CreateBoundingBox(center, tileSize / 2);
            var latitudeOffset = (bbox.MaxPoint.Latitude - bbox.MinPoint.Latitude) / resolution;
            var longitudeOffset = (bbox.MaxPoint.Longitude - bbox.MinPoint.Longitude) / resolution;
            var elevationProvider = new Mock<IElevationProvider>();
            elevationProvider.Setup(p => p.GetElevation(It.IsAny<double>(), It.IsAny<double>()))
                .Returns<double, double>((lat, lon) =>
            {
                var i = (int) Math.Round((lat - latitudeOffset/2 - bbox.MinPoint.Latitude) / latitudeOffset);
                var j = (int) Math.Round((lon - longitudeOffset / 2 - bbox.MinPoint.Longitude) / longitudeOffset);

                return i + j;
            });

            var provider = new HeightMapProvider(elevationProvider.Object);
            provider.Normalize = false;
            provider.DoSmooth = false;
            
            // ACT
            var heightMap = provider.GetHeightMap(center, resolution, tileSize);

            // ASSERT
            Assert.IsNotNull(heightMap);
            Assert.IsNotNull(heightMap.Data);
            
            var data = heightMap.Data;
            Assert.AreEqual(resolution, data.GetLength(0));
            Assert.AreEqual(resolution, data.GetLength(1));

            var cornerValue = (resolution - 1);
            Assert.AreEqual(0, data[0, 0]);
            Assert.AreEqual(cornerValue, data[resolution - 1, 0]);
            Assert.AreEqual(cornerValue, data[0, resolution - 1]);
            Assert.AreEqual(cornerValue * 2, data[resolution - 1, resolution - 1]);
        }

        [TestCase(257, 100, 52, 13)]
        [TestCase(513, 500, 52, 13)]
        [TestCase(1025, 1000, 52, 13)]
        public void CanLookupInTestData(int resolution, float tileSize, double latitude, double longitude)
        {
            
            // ARRANGE
            var center = new GeoCoordinate(latitude, longitude);
            var bbox = BoundingBox.CreateBoundingBox(center, tileSize / 2);
            var data = new float[resolution, resolution];

            for (int j = 0; j < resolution; j++)
                for(int i =0 ; i< resolution; i++)
                    data[j, i] = i + j;

            var maxElevation = resolution + resolution - 2;
            var heightMap = new HeightMap()
            {
                BoundingBox = bbox,
                Data = data,
                IsFlat = false,
                LatitudeOffset = (bbox.MaxPoint.Latitude - bbox.MinPoint.Latitude) / resolution,
                LongitudeOffset = (bbox.MaxPoint.Longitude - bbox.MinPoint.Longitude) / resolution,
                MaxElevation = maxElevation,
                Resolution = resolution,
                Size = tileSize
            };

            // ACT && ASSERT

            var cornerValue = resolution - 1;
            // center
            Assert.AreEqual((resolution + resolution - 2) / 2, heightMap.LookupHeight(center) / maxElevation);
            
            
            // left upper corner
            Assert.AreEqual(cornerValue, heightMap.LookupHeight(bbox.MinPoint.Latitude, bbox.MaxPoint.Longitude) / maxElevation);
            // left bottom corner
            Assert.AreEqual(0, heightMap.LookupHeight(bbox.MinPoint) / maxElevation);
            
            // right upper corner
            Assert.AreEqual(cornerValue * 2, heightMap.LookupHeight(bbox.MaxPoint) / maxElevation);
            // right bottom corner
            Assert.AreEqual(cornerValue, heightMap.LookupHeight(bbox.MaxPoint.Latitude, bbox.MinPoint.Longitude) / maxElevation);
        }
    }
}
