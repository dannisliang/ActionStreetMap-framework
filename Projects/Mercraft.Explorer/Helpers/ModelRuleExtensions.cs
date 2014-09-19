using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Utilities;
using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Explorer.Helpers
{
    public static class ModelRuleExtensions
    {
        public static Material GetMaterial(this Rule rule, IResourceProvider resourceProvider)
        {
            // TODO use resource loader
            var path = rule.Evaluate<string>("material");
            return resourceProvider.GetMatertial(@"Materials/" + path);
        }

        public static Color32 GetFillColor(this Rule rule)
        {
            var coreColor = rule.Evaluate<Core.Unity.Color32>("fill-color", ColorUtility.FromUnknown);
            return new Color32(coreColor.r, coreColor.g, coreColor.b, coreColor.a);
        }

        public static bool IsSkipped(this Rule rule)
        {
            return rule.EvaluateDefault("skip", false);
        }

        public static int GetLayerIndex(this Rule rule, int @default = -1)
        {
            return rule.EvaluateDefault("layer", @default);
        }

        public static bool IsRoad(this Rule rule)
        {
            return rule.EvaluateDefault("road", false);
        }

        public static bool IsElevation(this Rule rule)
        {
            return rule.EvaluateDefault("elevation", false);
        }

        public static bool IsTree(this Rule rule)
        {
            return rule.EvaluateDefault("tree", false);
        }

        public static bool IsTerrain(this Rule rule)
        {
            return rule.EvaluateDefault("terrain", false);
        }
    }
}
