using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Explorer.Builders;
using Mercraft.Explorer.Interactions;
using UnityEngine;

namespace Mercraft.Explorer.Helpers
{
    /// <summary>
    /// Provides methods for basic mapcss properties receiving
    /// </summary>
    public static class RuleExtensions
    {
        public static Material GetMaterial(this Rule rule, Model model)
        {
            var path =  rule.Evaluate<string>(model, "material");
            return Resources.Load<Material>(@"Materials/" + path);
        }

        public static float GetLevels(this Rule rule, Model model, int @default = 0)
        {
            return rule.EvaluateDefault(model, "levels", @default);
        }

        public static float GetHeight(this Rule rule, Model model)
        {
            return rule.Evaluate<float>(model, "height");
        }

        public static float GetMinHeight(this Rule rule, Model model, float defaultValue = 0)
        {
            return rule.EvaluateDefault<float>(model, "min_height", defaultValue);
        }

        public static IModelBuilder GetModelBuilder(this Rule rule, Model model, IEnumerable<IModelBuilder> builders)
        {
            var builderName = rule.Evaluate<string>(model, "builder");
            return builders.Single(mb => mb.Name == builderName);
        }

        public static IModelBehaviour GetModelBehaviour(this Rule rule, Model model, IEnumerable<IModelBehaviour> behaviours)
        {
            var builderName = rule.EvaluateDefault<string>(model, "behaviour", null);
            if (builderName == null)
                return null;
            return behaviours.Single(mb => mb.Name == builderName);
        }

        public static Color32 GetFillColor(this Rule rule, Model model, Color32 defaulColor)
        {
            return rule.EvaluateDefault(model, "fill-color", defaulColor);        
        }

        /// <summary>
        /// Z-index is just the lowest y coordinate
        /// </summary>
        public static float GetZIndex(this Rule rule, Model model)
        {
            return rule.Evaluate<float>(model, "z-index");      
        }

        /// <summary>
        /// Gets width
        /// </summary>
        public static float GetWidth(this Rule rule, Model model)
        {
            return rule.Evaluate<float>(model, "width");
        }
    }
}
