using System;
using System.Linq;
using Mercraft.Models.Buildings.Entities;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;

namespace Mercraft.Models.Buildings.Generators
{
    public class BuildingGenerator
    {
        public static void Generate(Model model, float height, int levels, bool generateRoof)
        {
            GenerateFloorPlan(model);

            RandomGenerator rgen = model.RanGen;
            BuildingStyle constraints = model.Style;

            model.FloorHeight = rgen.OutputRange(constraints.FloorHeight.Min, constraints.FloorHeight.Max);
            foreach (Volume volume in model.Plan.Volumes)
            {
                if (levels != 0)
                {
                    // level is defined but no height
                    if (Math.Abs(height) < float.Epsilon)
                    {
                        height = levels*model.FloorHeight;
                    }
                    else
                    {
                        // levels and height are defined - calculate floor height
                        model.FloorHeight = height/levels;
                    }
                    volume.Height = height;
                    volume.NumberOfFloors = levels;
                }
                else
                {
                    // no level, no height
                    if (Math.Abs(height) < float.Epsilon)
                    {
                        height = rgen.OutputRange(constraints.BoxSize.Min, constraints.BoxSize.Max);
                    }
                    volume.Height = height;
                    volume.NumberOfFloors = Mathf.FloorToInt(volume.Height / model.FloorHeight);
                }              
            }

            //texture generation
            GetTextures(model);

            //roof generation
            if (generateRoof)
                GenerateRoof(model);
        }

        private static void GenerateRoof(Model model)
        {
            var style = model.Style;
            var rgen = model.RanGen;
            var roofDesign = new Roof("default");

            roofDesign.Style = (RoofStyle) Enum.Parse(typeof(RoofStyle),
                style.RoofStyles[rgen.OutputRange(0, style.RoofStyles.Count - 1)], true);

            roofDesign.Height = rgen.OutputRange(style.RoofHeight.Min, style.RoofHeight.Max);
            roofDesign.FloorDepth = rgen.OutputRange(style.RoofFloorDepth.Min, style.RoofFloorDepth.Max);
            roofDesign.Depth = rgen.OutputRange(
                Mathf.Min(style.RoofFaceDepth.Min, roofDesign.FloorDepth),
                style.RoofFaceDepth.Max);

            roofDesign.HasDormers = rgen.Output <= style.DormerChance;
            roofDesign.DormerWidth = rgen.OutputRange(style.DormerWidth.Min, style.DormerWidth.Max);
            roofDesign.DormerHeight = rgen.OutputRange(style.DormerHeight.Min, Mathf.Min(roofDesign.Height, style.DormerHeight.Max));
            roofDesign.DormerRoofHeight = rgen.OutputRange(style.DormerRoofHeight.Min, style.DormerRoofHeight.Max);
            roofDesign.MinimumDormerSpacing = rgen.OutputRange(style.DormerSpacing.Min, style.DormerSpacing.Max);
            roofDesign.DormerHeightRatio = rgen.OutputRange(0.0f, 1.0f);

            roofDesign.Parapet = rgen.Output <= style.ParapetChance;
            roofDesign.ParapetDesignWidth = rgen.OutputRange(style.ParapetWidth.Min, style.ParapetWidth.Max);
            roofDesign.ParapetHeight = rgen.OutputRange(style.ParapetHeight.Min, style.ParapetHeight.Max);
            roofDesign.ParapetFrontDepth = rgen.OutputRange(style.ParapetFrontDepth.Min, style.ParapetFrontDepth.Max);
            roofDesign.ParapetBackDepth = rgen.OutputRange(style.ParapetBackDepth.Min, style.ParapetBackDepth.Max);

            model.Roofs.Add(roofDesign);
        }

        private static void GenerateFloorPlan(Model model)
        {
            var plan = new Plan();
            var bounds = model.Footprint;

            // TODO if a lot of points - split to volumes?
            plan.AddVolume(bounds.ToArray());
            model.Plan = plan;
        }

        private static void GetTextures(Model model)
        {
            var textureCache = model.TexturePack;

            RandomGenerator rgen = model.RanGen;

            var wallTexture = textureCache.Wall[rgen.OutputRange(0, textureCache.Wall.Count - 1)];
            wallTexture.MainTexture = Resources.Load<Texture2D>(wallTexture.Path);
            model.Textures.Add(wallTexture);


            var windowTexture = textureCache.Window[rgen.OutputRange(0, textureCache.Window.Count - 1)];
            windowTexture.MainTexture = Resources.Load<Texture2D>(windowTexture.Path);
            model.Textures.Add(windowTexture);

            var roofTexture = textureCache.Roof[rgen.OutputRange(0, textureCache.Roof.Count - 1)];
            roofTexture.MainTexture = Resources.Load<Texture2D>(roofTexture.Path);
            model.Textures.Add(roofTexture);

            var doorTexture = textureCache.Door[rgen.OutputRange(0, textureCache.Door.Count - 1)];
            doorTexture.MainTexture = Resources.Load<Texture2D>(doorTexture.Path);
            model.Textures.Add(doorTexture);
        }
    }
}