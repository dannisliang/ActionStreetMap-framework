using System.IO;
using System.Linq;
using System.Text;
using Mercraft.Maps.Core;
using Mercraft.Maps.Core.Projections;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Pbf;
using Mercraft.Maps.UI;
using Mercraft.Maps.UI.Scenes;
using Mercraft.Math.Primitives;
using NUnit.Framework;

namespace Mercraft.Maps.Osm.UnitTests
{
    [TestFixture]
    class SimpleTest
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
                    object sourceObject = source.Current();
                }
            }
        }

        [Test]
        public void CanGetOsmGeo()
        {
            var dataSource = GetDataSource();
            var box = CreateBox();

            var osmGeos = dataSource.Get(box, null);
           
            Assert.AreEqual(3856, osmGeos.Count);
        }

        [Test]
        public void CanFillScene()
        {
            var dataSource = GetDataSource();
            var box = CreateBox();

            var translator = new EmptyElementTranslator();

            IScene scene = new EmptyScene();
            var styleSceneManager = new SceneManager(scene, translator);

            styleSceneManager.FillScene(dataSource, box, new WebMercatorProjection());

            Assert.AreEqual(3856, translator.TranslatedOsmGeos.Count);
        }

        [Test]
        public void CanFillSmallScene()
        {
            var dataSource = GetDataSource();
            var box = CreateBox(200, 200, 51.26371, 4.7853, 19);
            var projection = new WebMercatorProjection();

            var translator = new EmptyElementTranslator();

            IScene scene = new EmptyScene();
            var styleSceneManager = new SceneManager(scene, translator);

            styleSceneManager.FillScene(dataSource, box, projection);

            Assert.AreEqual(36, translator.TranslatedOsmGeos.Count);
        }

        [Test]
        public void CanFillOneBuilding()
        {
            var dataSource = GetDataSource();
            //View2D view = CreateView(30, 30, 19, 51.26371, 4.7853);

            var box = CreateBox(30, 30, 51.26371, 4.7853, 19);
            var projection = new WebMercatorProjection();

            var translator = new EmptyElementTranslator();

            IScene scene = new EmptyScene();
            var styleSceneManager = new SceneManager(scene, translator);

            styleSceneManager.FillScene(dataSource, box, projection);

            Assert.AreEqual(2, translator.TranslatedOsmGeos.Count);
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

        private MemoryDataSource GetDataSource()
        {
            return MemoryDataSource
                .CreateFromPBFStream(new FileInfo(TestHelper.TestFilePath).OpenRead());
        }


        #endregion
    }
}
