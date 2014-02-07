using Mercraft.Maps.Core;
using Mercraft.Maps.Core.Projections;
using Mercraft.Math.Primitives;

namespace Mercraft.Maps.UnitTests
{
    internal static class TestHelper
    {
        public const string TestFilePath = @"..\..\..\..\Tests\TestAssets\kempen.osm.pbf";

        public static GeoCoordinateBox CreateBox(double height = 500, double width = 500, double latitude = 51.26371, double longitude = 4.7854, int zoomLevel = 16)
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

    }
}
