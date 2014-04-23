using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Primitives;

namespace Mercraft.Models.Buildings.Config
{
    /// <summary>
    /// Provides styles used by building logic
    /// </summary>
    public class BuildingStyleProvider
    {
        private readonly Dictionary<string, IList<BuildingStyle>> _themas;

        public BuildingStyleProvider(IConfigSection config)
        {   
            _themas = LoadThemas(config);
        }

        public BuildingStyle Get(string theme, string style)
        {
            return _themas[theme].Single(s => s.Name == style);
        }

        /// <summary>
        /// Loads themas from config
        /// </summary>
        private Dictionary<string, IList<BuildingStyle>> LoadThemas(IConfigSection config)
        {
            var themas = new Dictionary<string, IList<BuildingStyle>>();
            foreach (var themeConfig in config.GetSections("theme"))
            {
                var themeName = themeConfig.GetString("@name");
                var styles = new List<BuildingStyle>();
                foreach (var styleConfig in themeConfig.GetSections("style"))
                {
                    var style = new BuildingStyle();

                    style.Name = styleConfig.GetString("@name");
                    style.Texture = styleConfig.GetString("@texture");
                    style.BoxSize = GetRange(styleConfig.GetSection("box/height"));
                    style.FloorHeight = GetRange(styleConfig.GetSection("floor/height"));

                    style.BayHeight = GetRange(styleConfig.GetSection("bay/height"));
                    style.BayWidth = GetRange(styleConfig.GetSection("bay/width"));
                    style.BayDepth = GetRange(styleConfig.GetSection("bay/depth"));
                    style.BaySpacing = GetRange(styleConfig.GetSection("bay/spacing"));

                    style.FacadeDepth = GetRange(styleConfig.GetSection("facade/depth"));

                    style.RoofHeight = GetRange(styleConfig.GetSection("roof/height"));
                    style.RoofFaceDepth = GetRange(styleConfig.GetSection("roof/faceDepth"));
                    style.RoofFloorDepth = GetRange(styleConfig.GetSection("roof/floorDepth"));

                    style.RoofStyles = new List<string>(8);
                    foreach (var configSection in styleConfig.GetSections("roof/styles/include"))
                    {
                        style.RoofStyles.Add(configSection.GetString("@type"));
                    }

                    style.ParapetChance = styleConfig.GetFloat("roof/parapet/@chance");
                    style.ParapetWidth = GetRange(styleConfig.GetSection("roof/parapet/width"));
                    style.ParapetHeight = GetRange(styleConfig.GetSection("roof/parapet/height"));
                    style.ParapetFrontDepth = GetRange(styleConfig.GetSection("roof/parapet/frontDepth"));
                    style.ParapetBackDepth = GetRange(styleConfig.GetSection("roof/parapet/backDepth"));

                    style.DormerChance = styleConfig.GetFloat("roof/dormer/@chance");
                    style.DormerWidth = GetRange(styleConfig.GetSection("roof/dormer/width"));
                    style.DormerHeight = GetRange(styleConfig.GetSection("roof/dormer/height"));
                    style.DormerSpacing = GetRange(styleConfig.GetSection("roof/dormer/spacing"));
                    style.DormerRoofHeight = GetRange(styleConfig.GetSection("roof/dormer/roofHeight"));

                    styles.Add(style);
                }
                themas.Add(themeName, styles);
            }
            return themas;
        }

        private Range<float> GetRange(IConfigSection section)
        {
            return new Range<float>(section.GetFloat("@min"), section.GetFloat("@max"));
        }        
    }
}
