namespace ActionStreetMap.Core.Elevation
{
    /// <summary>
    ///     Defines behavior of elevation provider.
    /// </summary>
    public interface IElevationProvider
    {
        /// <summary>
        ///     Gets elevation for given latitude and longitude.
        /// </summary>
        /// <param name="latitude">Latitude.</param>
        /// <param name="longitude">Longitude.</param>
        /// <returns>Elevation.</returns>
        float GetElevation(double latitude, double longitude);

        /// <summary>
        ///     Gets elevation for given geo coordinate.
        /// </summary>
        /// <param name="geoCoordinate">Geo coordinate.</param>
        /// <returns>Elevation.</returns>
        float GetElevation(GeoCoordinate geoCoordinate);
    }
}
