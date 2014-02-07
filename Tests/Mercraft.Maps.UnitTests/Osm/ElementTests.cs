using System.IO;
using Mercraft.Maps.Core;
using Mercraft.Maps.Core.Projections;
using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Pbf;
using Mercraft.Math.Primitives;
using Mercraft.Models;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Osm
{
    [TestFixture]
    class ElementTests
    {
        [Test]
        public void CanPullPbfStream()
        {
            using (Stream stream = new FileStream(TestHelper.TestFilePath, FileMode.Open))
            {
                var source = new PbfOsmStreamSource(stream);
                source.Initialize();
                // pulling
                while (source.MoveNext())
                {
                    Element element = source.Current();
                }
            }
        }

        [Test]
        public void CanGetOsmGeo()
        {
            using (Stream stream = new FileInfo(TestHelper.TestFilePath).OpenRead())
            {
                 var dataSource = MemoryDataSource.CreateFromPBFStream(stream);
                 var box = CreateBox();

                 var osmGeos = dataSource.Get(box, null);

                 Assert.AreEqual(3856, osmGeos.Count);
            }           
        }

        [Test]
        public void CanFillScene()
        {
            using (Stream stream = new FileInfo(TestHelper.TestFilePath).OpenRead())
            {
                var dataSource = MemoryDataSource.CreateFromPBFStream(stream);
                var box = CreateBox();

                var visitor = new CountableElementVisitor();

                var elementManager = new ElementManager(visitor);

                elementManager.FillBoundingBox(dataSource, box);

                Assert.AreEqual(3856, visitor.Elements.Count);
            }               
        }

        [Test]
        public void CanFillSmallScene()
        {
            using (Stream stream = new FileInfo(TestHelper.TestFilePath).OpenRead())
            {
                var dataSource = MemoryDataSource.CreateFromPBFStream(stream);
                var box = CreateBox(200, 200, 51.26371, 4.7853, 19);

                var visitor = new CountableElementVisitor();

                var elementManager = new ElementManager(visitor);

                elementManager.FillBoundingBox(dataSource, box);

                Assert.AreEqual(36, visitor.Elements.Count);
            }
        }

        [Test]
        public void CanFillOneBuilding()
        {
            using (Stream stream = new FileInfo(TestHelper.TestFilePath).OpenRead())
            {
                var dataSource = MemoryDataSource.CreateFromPBFStream(stream);

                var box = CreateBox(30, 30, 51.26371, 4.7853, 19);

                var visitor = new CountableElementVisitor();

                var elementManager = new ElementManager(visitor);

                elementManager.FillBoundingBox(dataSource, box);

                Assert.AreEqual(2, visitor.Elements.Count);
            }
        }


        #region Helpers

        private GeoCoordinateBox CreateBox(double height = 500, double width = 500, double latitude = 51.26371, double longitude = 4.7854, int zoomLevel = 16)
        {
            const int DefaultZoom = 15;
            IProjection projection = new WebMercatorProjection();
            bool xInverted = false;
            bool yInverted = false;
            double realZoom = System.Math.Pow(2, zoomLevel - DefaultZoom) * 256.0; ;

            width = width / realZoom;
            height = height / realZoom;

            int angleY = 0;

            double[] sceneCenter = projection.ToPixel(latitude, longitude);

            var rectangle = RectangleF2D.FromBoundsAndCenter(width, height,
                (float)sceneCenter[0], (float)sceneCenter[1], angleY);

            var boundingBox = rectangle.BoundingBox;

            return new GeoCoordinateBox(
                projection.ToGeoCoordinates(boundingBox.Min[0], boundingBox.Min[1]),
                projection.ToGeoCoordinates(boundingBox.Max[0], boundingBox.Max[1]));
        }

        #endregion
    }
}
