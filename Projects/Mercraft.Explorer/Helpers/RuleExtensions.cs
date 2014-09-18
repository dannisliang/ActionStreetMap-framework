using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Utilities;
using Mercraft.Explorer.Scene;
using Mercraft.Models.Terrain;
using Mercraft.Models.Utils;
using UnityEngine;


namespace Mercraft.Explorer.Helpers
{
    /// <summary>
    /// Provides methods for basic mapcss properties receiving
    /// </summary>
    public static class RuleExtensions
    {
        public static Material GetMaterial(this Rule rule, IResourceProvider resourceProvider)
        {
            // TODO use resource loader
            var path =  rule.Evaluate<string>("material");
            return resourceProvider.GetMatertial(@"Materials/" + path);
        }

        public static int GetLevels(this Rule rule, int @default = 0)
        {
            return rule.EvaluateDefault("levels", @default);
        }

        public static float GetHeight(this Rule rule, float defaultValue = 0)
        {
            return rule.EvaluateDefault<float>("height", defaultValue);
        }

        public static float GetMinHeight(this Rule rule, float defaultValue = 0)
        {
            return rule.EvaluateDefault<float>("min_height", defaultValue);
        }

        public static string GetBuildingType(this Rule rule)
        {
            return rule.Evaluate<string>("building-style");
        }

        /*public static IEnumerable<IModelBuilder> GetModelBuilders(this Rule rule, IEnumerable<IModelBuilder> builders)
        {
            var builderNames = rule.Evaluate<List<string>>("builder");
            if (builderNames == null)
                return null;
            return builders.Where(mb => builderNames.Contains(mb.Name));
        }*/

        public static IModelBuilder GetModelBuilder(this Rule rule, IEnumerable<IModelBuilder> builders)
        {
            var builderName = rule.EvaluateDefault<string>("builder", null);
            if (builderName == null)
                return null;
            return builders.Single(mb => mb.Name == builderName);
        }

        public static IModelBehaviour GetModelBehaviour(this Rule rule, IEnumerable<IModelBehaviour> behaviours)
        {
            var builderName = rule.EvaluateDefault<string>("behaviour", null);
            if (builderName == null)
                return null;
            return behaviours.Single(mb => mb.Name == builderName);
        }

        public static bool IsSkipped(this Rule rule)
        {
            return rule.EvaluateDefault("skip", false);
        }

        public static Color32 GetFillColor(this Rule rule)
        {
            var coreColor = rule.Evaluate<Core.Unity.Color32>("fill-color", ColorUtility.FromUnknown);
            return new Color32(coreColor.r, coreColor.g, coreColor.b, coreColor.a);
        }

        public static int GetLayerIndex(this Rule rule, int @default = -1)
        {
            return rule.EvaluateDefault("layer", @default);
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

        public static int GetSplatIndex(this Rule rule)
        {
            return rule.Evaluate<int>("splat");
        }

        public static int GetTerrainDetailIndex(this Rule rule)
        {
            return rule.EvaluateDefault<int>("terrainDetail", AreaSettings.DefaultIndex);
        }

        public static List<List<string>> GetSplatParams(this Rule rule)
        {
            return rule.EvaluateList<List<string>>("splat");
        }

        public static List<List<string>> GetDetailParams(this Rule rule)
        {
            return rule.EvaluateList<List<string>>("detail");
        }

        public static int GetResolution(this Rule rule)
        {
            return rule.Evaluate<int>("resolution"); 
        }

        public static int GetHeightMapSize(this Rule rule)
        {
            return rule.Evaluate<int>("heightmapsize");
        }

        public static int GetPixelMapError(this Rule rule)
        {
            return rule.Evaluate<int>("pixelMapError");
        }

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
