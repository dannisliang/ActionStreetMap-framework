using System;
using System.Linq;
using Mercraft.Models.Buildings.Entities;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;
using Texture = Mercraft.Models.Buildings.Entities.Texture;

namespace Mercraft.Models.Buildings
{
    public class BuildingGenerator
    {
        private class TextureContext
        {
            public Texture Wall;
            public Texture Window;
            public Texture Door;
            public Texture Roof;
        }

        public static void Generate(Data data, float height, int levels)
        {
            var textureContext = new TextureContext();
            GenerateFloorPlan(data);

            RandomGenerator rgen = data.RanGen;
            BuildingStyle constraints = data.Style;

            data.FloorHeight = rgen.OutputRange(constraints.FloorHeight.Min, constraints.FloorHeight.Max);
            foreach (Volume volume in data.Plan.Volumes)
            {
                if (levels != 0)
                {
                    // level is defined but no height
                    if (Math.Abs(height) < float.Epsilon)
                    {
                        height = levels*data.FloorHeight;
                    }
                    else
                    {
                        // levels and height are defined - calculate floor height
                        data.FloorHeight = height/levels;
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
                    volume.NumberOfFloors = Mathf.FloorToInt(volume.Height / data.FloorHeight);
                }              
            }

            //texture generation
            GetTextures(data, textureContext);

            //facade generation
            GenerateFacades(data, textureContext);

            //roof generation
            GenerateRoof(data);

            // TODO generate details
        }

        private static void GenerateRoof(Data data)
        {
            var style = data.Style;
            var rgen = data.RanGen;
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

            data.Roofs.Add(roofDesign);
        }

        private static void GenerateFacades(Data data, TextureContext textureContext)
        {
            var style = data.Style;
            var rgen = data.RanGen;

            //generate bays
            //blank
            Bay blankBay = new Bay("Blank");
            blankBay.IsOpening = false;
            data.Bays.Add(blankBay);
           
            //door
            Bay doorBay = new Bay("Door");
            doorBay.OpeningHeight = data.FloorHeight * 0.9f;
            doorBay.OpeningHeightRatio = 0.0f;
            float doorWidth = (textureContext.Door.MainTexture.width / (float)textureContext.Door.MainTexture.height) * doorBay.OpeningHeight;
            doorBay.OpeningWidth = doorWidth;
            doorBay.OpeningDepth = rgen.OutputRange(0.0f, 0.3f);
            doorBay.SetTexture(Bay.BayTextureName.OpeningBack, data.Textures.IndexOf(textureContext.Door));
            data.Bays.Add(doorBay);

            GenerateGroundFloor(data, textureContext);


            //generate facades
            Facade basicFacade = new Facade("default");
            basicFacade.SimpleBay.OpeningWidth = rgen.OutputRange(style.BayWidth.Min,style.BayWidth.Max);
            basicFacade.SimpleBay.OpeningHeight = rgen.OutputRange(style.BayHeight.Min, Mathf.Min(data.FloorHeight, style.BayHeight.Max));
            basicFacade.SimpleBay.OpeningDepth = rgen.OutputRange(style.BayDepth.Min, style.BayDepth.Max);

            basicFacade.SimpleBay.Spacing = rgen.OutputRange(style.BaySpacing.Min, style.BaySpacing.Max);
            data.Facades.Add(basicFacade);


            //ground floor with and without door
            Facade groundFloorDoor = new Facade("Ground Floor With Door");
            groundFloorDoor.IsPatterned = true;
            int patternSize = rgen.OutputRange(1, 8);
            for (int i = 0; i < patternSize; i++)
            {
                groundFloorDoor.BayPattern.Add(rgen.Output > 0.2f ? 2 : 0);
            }          
            groundFloorDoor.BayPattern.Insert(rgen.OutputRange(0, patternSize), 1); //insert door into pattern
            
            
            data.Facades.Add(groundFloorDoor);

            Plan plan = data.Plan;
            for (int v = 0; v < plan.Volumes.Count; v++)
            {
                Volume volume = plan.Volumes[v];
                int numberOfFloors = volume.NumberOfFloors;
                volume.Style.Clear();
                for (int f = 0; f < volume.Points.Count; f++)
                {
                    int facadeIndex = volume.Points[f];
                    volume.Style.AddStyle(0, facadeIndex, numberOfFloors - 1);
                    volume.Style.AddStyle(1, facadeIndex, 1);
                }
            }
        }

        private static void GenerateGroundFloor(Data data, TextureContext textureContext)
        {
            var style = data.Style;
            var rgen = data.RanGen;


            //ground window
            Bay groundWindow = new Bay("Ground Window");
            groundWindow.OpeningWidth = rgen.OutputRange(style.BayWidth.Min, style.BayWidth.Max);
            groundWindow.OpeningHeight = rgen.OutputRange(style.BayHeight.Min, Mathf.Min(data.FloorHeight, style.BayHeight.Max));
            groundWindow.OpeningDepth = rgen.OutputRange(style.BayDepth.Min, style.BayDepth.Max);
            groundWindow.OpeningHeightRatio = 0.8f;
            data.Bays.Add(groundWindow);
            

            Texture groundFloorWindowTexture = textureContext.Window.Duplicate("groundWindowTexture");
            groundFloorWindowTexture.Tiled = false;
            groundFloorWindowTexture.TiledX = Mathf.RoundToInt(groundWindow.OpeningWidth / groundWindow.OpeningHeight);
            int groundtextureIndex = data.Textures.Count;
            data.Textures.Add(groundFloorWindowTexture);
            groundWindow.SetTexture(Bay.BayTextureName.OpeningBack, groundtextureIndex);
            //other windows
            Bay windowBay = new Bay("Window");
            data.Bays.Add(windowBay);
            //util window
            Bay utilBay = new Bay("Utility Window");
            data.Bays.Add(utilBay);
        }

        private static void GenerateFloorPlan(Data data)
        {
            var plan = new Plan();
            var bounds = data.Footprint;

            // TODO if a lot of points - split to volumes?
            plan.AddVolume(bounds.ToArray());
            data.Plan = plan;
        }

        private static void GetTextures(Data data, TextureContext textureContext)
        {
            var textureCache = data.TexturePack;

            RandomGenerator rgen = data.RanGen;

            var wallTexture = textureCache.Wall[rgen.OutputRange(0, textureCache.Wall.Count - 1)];
            wallTexture.MainTexture = Resources.Load<Texture2D>(wallTexture.Path);
            textureContext.Wall = wallTexture;
            data.Textures.Add(textureContext.Wall = wallTexture);


            var windowTexture = textureCache.Window[rgen.OutputRange(0, textureCache.Window.Count - 1)];
            windowTexture.MainTexture = Resources.Load<Texture2D>(windowTexture.Path);
            data.Textures.Add(textureContext.Window = windowTexture);

            var roofTexture = textureCache.Roof[rgen.OutputRange(0, textureCache.Roof.Count - 1)];
            roofTexture.MainTexture = Resources.Load<Texture2D>(roofTexture.Path);
            data.Textures.Add(textureContext.Roof = roofTexture);

            var doorTexture = textureCache.Door[rgen.OutputRange(0, textureCache.Door.Count - 1)];
            doorTexture.MainTexture = Resources.Load<Texture2D>(doorTexture.Path);
            data.Textures.Add(textureContext.Door = doorTexture);
        }
    }
}