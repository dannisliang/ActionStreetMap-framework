using System;
using UnityEngine;

namespace Mercraft.Models.Algorithms
{

    public static class GeoProjection
    {
        /// <summary>
        /// http://stackoverflow.com/questions/3024404/transform-longitude-latitude-into-meters?rq=1
        /// </summary>
        public static Vector2 ToMapCoordinates(MapPoint relativeNullPoint, MapPoint p)
        {
            // The circumference at the equator (latitude 0)
            const int latitudeEquator = 40075160;

            //distance of full circle around the earth through the poles
            const int circleDistance = 40008000;

            double deltaLatitude = p.Latitude - relativeNullPoint.Latitude;
            double deltaLongitude = p.Longitude - relativeNullPoint.Longitude;
            double latitudeCircumference = latitudeEquator * Math.Cos(MathUtility.Deg2Rad(relativeNullPoint.Latitude));
            double resultX = deltaLongitude * latitudeCircumference / 360;
            double resultY = deltaLatitude * circleDistance / 360;
            
            return new Vector2((float)resultX, (float) resultY);
        }

    }
}
