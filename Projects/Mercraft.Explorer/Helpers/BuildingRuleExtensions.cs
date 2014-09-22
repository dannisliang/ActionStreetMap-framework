using Mercraft.Core.MapCss.Domain;

namespace Mercraft.Explorer.Helpers
{
    public static class BuildingRuleExtensions
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
            return rule.EvaluateDefault<string>("building-material", @default);
        }

        public static string GetRoofType(this Rule rule, string @default = null)
        {
            return rule.EvaluateDefault<string>("roof-type", @default);
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
