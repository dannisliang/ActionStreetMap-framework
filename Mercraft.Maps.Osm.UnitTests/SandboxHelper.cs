using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mercraft.Maps.Core;
using Mercraft.Maps.Core.Projections;
using Mercraft.Maps.UI.Rendering;
using Mercraft.Math.Primitives;
using Mercraft.Math.Units.Angle;

namespace Mercraft.Maps.Osm.UnitTests
{
    public static class SandboxHelper
    {

        public static View2D CreateView(GeoCoordinate center, float width, float height, float zoom,
            Degree angle, bool xInverted, bool yInverted)
        {
            // get the projection.
            IProjection projection = new WebMercatorProjection();

            // calculate the center/zoom in scene coordinates.
            double[] sceneCenter = projection.ToPixel(center.Latitude, center.Longitude);
            float sceneZoomFactor = (float) projection.ToZoomFactor(zoom);

            // inversion flags for view: only invert when different.
            bool invertX = xInverted && (xInverted != !projection.DirectionX);
            bool invertY = yInverted && (yInverted != !projection.DirectionY);

            return View2D.CreateFrom((float)sceneCenter[0], (float)sceneCenter[1],
                                             width, height, sceneZoomFactor,
                                             invertX, invertY, angle);
        }

        public static GeoCoordinateBox CreateBox(View2D view)
        {
            IProjection projection = new WebMercatorProjection();
            //{RectF:[(16819.08984375,10931.2509765625),(16820.06640625,10932.2275390625)]}
            var viewBox = view.OuterBox;
            
            return new GeoCoordinateBox(projection.ToGeoCoordinates(viewBox.Min[0], viewBox.Min[1]),
                                            projection.ToGeoCoordinates(viewBox.Max[0], viewBox.Max[1]));
        }
       
    }
}
