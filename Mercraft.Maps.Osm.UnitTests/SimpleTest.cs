using System.IO;
using System.Linq;
using System.Text;
using Mercraft.Maps.Core;
using Mercraft.Maps.Core.Projections;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Pbf;
using Mercraft.Maps.UI;
using Mercraft.Maps.UI.Rendering;
using Mercraft.Maps.UI.Scenes;

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
                var target = new EmptyOsmStreamTarget(source);

                target.Pull();
            }
        }

        [Test]
        public void CanGetOsmGeo()
        {
            var dataSource = GetDataSource();
            var box = GetBox();

            var osmGeos = dataSource.Get(box, null);
           
            Assert.AreEqual(3856, osmGeos.Count);
        }

        [Test]
        public void CanFillScene()
        {
            var dataSource = GetDataSource();
            var box = GetBox();

            var translator = new EmptyStyleTranslator();

            IScene scene = new EmptyScene();
            var styleSceneManager = new StyleSceneManager(scene, translator);

            styleSceneManager.FillScene(dataSource, box, new WebMercatorProjection());

            Assert.AreEqual(3856, translator.TranslatedOsmGeos.Count);
        }

        [Test]
        public void CanFillSmallScene()
        {
            var dataSource = GetDataSource();
            View2D view = CreateView(200,200, 19);
            var box = SandboxHelper.CreateBox(view);
            var projection = new WebMercatorProjection();

            var translator = new EmptyStyleTranslator();

            IScene scene = new EmptyScene();
            var styleSceneManager = new StyleSceneManager(scene, translator);

            styleSceneManager.FillScene(dataSource, box, projection);

            Assert.AreEqual(38, translator.TranslatedOsmGeos.Count);
        }

        [Test]
        public void CanFillOneBuilding()
        {
            var dataSource = GetDataSource();
            View2D view = CreateView(30, 30, 19, 51.26371, 4.7853);
            var box = SandboxHelper.CreateBox(view);
            var projection = new WebMercatorProjection();

            var translator = new EmptyStyleTranslator();

            IScene scene = new EmptyScene();
            var styleSceneManager = new StyleSceneManager(scene, translator);

            styleSceneManager.FillScene(dataSource, box, projection);

            Assert.AreEqual(2, translator.TranslatedOsmGeos.Count);
        }


        #region Helpers

        private IDataSourceReadOnly GetDataSource()
        {
            return MemoryDataSource
                .CreateFromPBFStream(new FileInfo(TestHelper.TestFilePath).OpenRead());
        }

        public View2D CreateView(float height = 500, float width = 500, float zoom = 16,
            double latitude = 51.26371, double longitude = 4.7854)
        {
            var center = new GeoCoordinate(latitude, longitude);
            // {RectF:[(16819.08984375,10931.2509765625),(16820.06640625,10932.2275390625)]}
            return SandboxHelper.CreateView(center, height, width, zoom, 0, false, true);
        }

        private GeoCoordinateBox GetBox()
        {
            var view = CreateView();
            return SandboxHelper.CreateBox(view);
        }

        #endregion
    }
}
