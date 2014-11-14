using ActionStreetMap.Core.MapCss.Domain;
using ActionStreetMap.Core.Utilities;

namespace ActionStreetMap.Explorer.Helpers
{
    internal static class BuildingRuleExtensions
    {
        public static float GetMinHeight(this Rule rule, float defaultValue = 0)
        {
            return rule.EvaluateDefault<float>("min_height", defaultValue);
        }

        public static string GetBuildingType(this Rule rule)
        {
            return rule.Evaluate<string>("building-style");
        }

        public static string GetFacadeMaterial(this Rule rule, string @default = null)
        {
            return rule.EvaluateDefault<string>("facade-material", @default);
        }

        public static string GetRoofType(this Rule rule, string @default = null)
        {
            return rule.EvaluateDefault<string>("roof-style", @default);
        }

        public static Core.Unity.Color32 GetRoofColor(this Rule rule)
        {
            return rule.Evaluate<Core.Unity.Color32>("roof-color", ColorUtility.FromUnknown);
        }

        public static float GetRoofHeight(this Rule rule, float defaultValue = 0)
        {
            return rule.EvaluateDefault<float>("roof-height", defaultValue);
        }

        public static string GetRoofMaterial(this Rule rule, string @default = null)
        {
            return rule.EvaluateDefault<string>("roof-material", @default);
        }

        public static int GetLevels(this Rule rule, int @default = 0)
        {
            return rule.EvaluateDefault("levels", @default);
        }

        public static bool IsPart(this Rule rule)
        {
            return rule.EvaluateDefault("part", false);
        }
    }
}
