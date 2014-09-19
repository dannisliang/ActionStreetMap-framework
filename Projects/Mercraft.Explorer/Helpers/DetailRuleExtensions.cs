using Mercraft.Core.MapCss.Domain;

namespace Mercraft.Explorer.Helpers
{
    public static class DetailRuleExtensions
    {
        public static string GetDetail(this Rule rule)
        {
            return rule.Evaluate<string>("detail");
        }

        public static bool IsRoadFix(this Rule rule)
        {
            return rule.EvaluateDefault("roadFix", false);
        }

        public static float GetDetailRotation(this Rule rule)
        {
            return rule.EvaluateDefault<float>("rotation", 0);
        }
    }
}
