namespace ActionStreetMap.Models
{
    internal static class Strings
    {
        public static string CannotFindRoofBuilder =
            "Cannot find roof builder which can build roof of given building: {0} - suspect wrong theme definition";

        public static string CannotClipPolygon =
            "The polygons passed in must have at least 3 MapPoints: subject={0}, clip={1}";

        public static string BugInPolygonOrderAlgorithm = "Bug in polygon order algorithm!";

        public static string GabledRoofGenFailed = "Gabled roof generation algorithm is faled for {0}";
    }
}
