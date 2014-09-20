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
            return rule.Evaluate<string>("building");
        }

        public static int GetLevels(this Rule rule, int @default = 0)
        {
            return rule.EvaluateDefault("levels", @default);
        }

    }
}
