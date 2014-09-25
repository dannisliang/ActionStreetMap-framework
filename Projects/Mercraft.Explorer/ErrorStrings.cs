namespace Mercraft.Explorer
{
    internal static class ErrorStrings
    {
        public static string CannotGetBuildingStyle = "Can't get building style - unknown building type: {0}. " +
                                                      "Try to check your current mapcss and theme files";

        
        public static string InvalidPolyline = "Attempt to render polyline with less than 2 points";
    }
}
