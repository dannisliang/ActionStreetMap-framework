namespace Mercraft.Core.Elevation
{
    public interface IElevationProvider
    {
        float GetElevation(double latitude, double longitude);
        float GetElevation(GeoCoordinate geoCoordinate);
    }
}
