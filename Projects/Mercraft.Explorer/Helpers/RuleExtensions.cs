using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using UnityEngine;

namespace Mercraft.Explorer.Helpers
{
    public static class RuleExtensions
    {
        public static Material GetMaterial(this Rule rule, Model model)
        {
            var path =  rule.Evaluate<string>(model, "material");
            return Resources.Load<Material>(@"Materials/" + path);
        }
    }
}
