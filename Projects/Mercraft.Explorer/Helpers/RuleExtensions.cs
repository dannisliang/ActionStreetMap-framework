using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using UnityEngine;

namespace Mercraft.Explorer.Helpers
{
    /// <summary>
    /// Provides methods for basic properties
    /// </summary>
    public static class RuleExtensions
    {
        public static Material GetMaterial(this Rule rule, Model model)
        {
            var path =  rule.Evaluate<string>(model, "material");
            return Resources.Load<Material>(@"Materials/" + path);
        }

        public static int GetHeight(this Rule rule, Model model)
        {
            return rule.Evaluate<int>(model, "height");
        }
    }
}
