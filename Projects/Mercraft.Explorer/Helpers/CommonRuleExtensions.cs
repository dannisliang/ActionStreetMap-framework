using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Explorer.Scene;

namespace Mercraft.Explorer.Helpers
{
    /// <summary>
    /// Provides methods for basic mapcss properties receiving
    /// </summary>
    public static class CommonRuleExtensions
    {
        public static string GetKey(this Rule rule)
        {
            return rule.Evaluate<string>("key");
        }

        public static float GetHeight(this Rule rule, float defaultValue = 0)
        {
            return rule.EvaluateDefault<float>("height", defaultValue);
        }

        /*public static IEnumerable<IModelBuilder> GetModelBuilders(this Rule rule, IEnumerable<IModelBuilder> builders)
        {
            var builderNames = rule.Evaluate<List<string>>("builder");
            if (builderNames == null)
                return null;
            return builders.Where(mb => builderNames.Contains(mb.Name));
        }*/

        public static IModelBuilder GetModelBuilder(this Rule rule, IModelBuilder[] builders)
        {
            var builderName = rule.EvaluateDefault<string>("builder", null);
            if (builderName == null)
                return null;
            // NOTE use for to avoid allocations
            for (int i = 0; i< builders.Length; i++)
                if (builders[i].Name == builderName)
                    return builders[i];
            return null;
        }

        public static IModelBehaviour GetModelBehaviour(this Rule rule, IModelBehaviour[] behaviours)
        {
            var behaviorName = rule.EvaluateDefault<string>("behaviour", null);
            if (behaviorName == null)
                return null;
            // NOTE use for to avoid allocations
            for (int i = 0; i < behaviours.Length; i++)
                if (behaviours[i].Name == behaviorName)
                    return behaviours[i];
            return null;
        }

        /// <summary>
        /// Z-index is just the lowest y coordinate
        /// </summary>
        public static float GetZIndex(this Rule rule)
        {
            return rule.Evaluate<float>("z-index");      
        }

        /// <summary>
        /// Gets width
        /// </summary>
        public static float GetWidth(this Rule rule)
        {
            return rule.Evaluate<float>("width");
        }
    }
}
