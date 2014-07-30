using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Infrastructure.Config;
using Mercraft.Models.Buildings;
using Mercraft.Models.Buildings.Facades;
using Mercraft.Models.Buildings.Roofs;
using Mercraft.Models.Roads;
using UnityEngine;

namespace Mercraft.Explorer.Themes
{
    public interface IThemeProvider
    {
        Theme Get();
        Theme Get(string name);
    }

    public class ThemeProvider: IThemeProvider, IConfigurable
    {
        private string _defaultThemeName;
        private const string ThemesKey = "themes/theme";
        private const string FacadeBuildersKey = "scene/world/buildings/builders/facades";
        private const string RoofBuildersKey = "scene/world/buildings/builders/roofs";
        
        private Dictionary<string, Theme> _themes;

        public Theme Get()
        {
            return Get(_defaultThemeName);
        }

        public Theme Get(string name)
        {
            return _themes[name];
        }

        public void Configure(IConfigSection configSection)
        {
            var facadeBuilderMap = LoadFacadeBuilderMap(configSection.GetSection(FacadeBuildersKey));
            var roofBuilderMap = LoadRoofBuilderMap(configSection.GetSection(RoofBuildersKey));
            _themes = new Dictionary<string, Theme>();
            foreach (var themeConfig in configSection.GetSections(ThemesKey))
            {
                var theme = new Theme();
                theme.Name = themeConfig.GetString("@name");
                theme.BuildingTypeStyleMapping = new Dictionary<string, List<BuildingStyle>>();
                theme.RoadTypeStyleMapping = new Dictionary<string, List<RoadStyle>>();
                
                // set default theme name
                if (string.IsNullOrEmpty(_defaultThemeName))
                    _defaultThemeName = theme.Name;

                ConfigureBuildings(themeConfig, theme, facadeBuilderMap, roofBuilderMap);
                ConfigureRoads(themeConfig, theme);

                _themes.Add(theme.Name, theme);
            }
        }

        #region Buildings

        private void ConfigureBuildings(IConfigSection themeConfig, Theme theme,
            Dictionary<string, IFacadeBuilder> facadeBuilderMap, Dictionary<string, IRoofBuilder> roofBuilderMap)
        {
            var textureMap = LoadBuildingTextureMap(themeConfig);

            foreach (var buildingTypeConfig in themeConfig.GetSections("buildings/types/type"))
            {
                var typeName = buildingTypeConfig.GetString("@name");
                var styles = new List<BuildingStyle>();
                foreach (var buildingStyleConfig in buildingTypeConfig.GetSections("style"))
                {
                    styles.Add(new BuildingStyle()
                    {
                        Texture = buildingStyleConfig.GetString("texture"),
                        Material = buildingStyleConfig.GetString("material"),
                        Floors = buildingStyleConfig.GetInt("floors"),
                        UvMap = textureMap[buildingStyleConfig.GetInt("textureMap/@index")],
                        FacadeBuilder = facadeBuilderMap[buildingStyleConfig.GetString("facade/@builder")],
                        RoofBuilder = roofBuilderMap[buildingStyleConfig.GetString("roof/@builder")]
                    });
                }
                theme.BuildingTypeStyleMapping.Add(typeName, styles);
            }
        }

        private Dictionary<string, IFacadeBuilder> LoadFacadeBuilderMap(IConfigSection facadeBuilderMapConfig)
        {
            var map = new Dictionary<string, IFacadeBuilder>();
            foreach (var facadeBuilderConfig in facadeBuilderMapConfig.GetSections("include"))
            {
                map.Add(facadeBuilderConfig.GetString("@name"),
                    facadeBuilderConfig.GetInstance<IFacadeBuilder>("@type"));
            }
            return map;
        }

        private Dictionary<string, IRoofBuilder> LoadRoofBuilderMap(IConfigSection roofBuilderMap)
        {
            var map = new Dictionary<string, IRoofBuilder>();
            foreach (var roofBuilderConfig in roofBuilderMap.GetSections("include"))
            {
                map.Add(roofBuilderConfig.GetString("@name"),
                    roofBuilderConfig.GetInstance<IRoofBuilder>("@type"));
            }
            return map;
        }

        private Dictionary<int, BuildingStyle.TextureUvMap> LoadBuildingTextureMap(IConfigSection textureMapConfig)
        {
            var textureMaps = new Dictionary<int, BuildingStyle.TextureUvMap>();
            foreach (var uvConfig in textureMapConfig.GetSections("buildings/textureMap/uv"))
            {
                var index = uvConfig.GetInt("@index");
                textureMaps.Add(index, new BuildingStyle.TextureUvMap()
                {
                    Front = GetUv(uvConfig.GetSection("facade/front")).ToArray(),
                    Back = GetUv(uvConfig.GetSection("facade/back")).ToArray(),
                    Side = GetUv(uvConfig.GetSection("facade/side")).ToArray(),
                    Roof = GetUv(uvConfig.GetSection("roof")).ToArray(),
                });
            }
            return textureMaps;
        }

        #endregion

        #region Roads

        private void ConfigureRoads(IConfigSection themeConfig, Theme theme)
        {
            var textureMap = LoadRoadTextureMap(themeConfig);
            foreach (var buildingTypeConfig in themeConfig.GetSections("roads/types/type"))
            {
                var typeName = buildingTypeConfig.GetString("@name");
                var styles = new List<RoadStyle>();
                foreach (var roadStyleConfig in buildingTypeConfig.GetSections("style"))
                {
                    styles.Add(new RoadStyle()
                    {
                        Texture = roadStyleConfig.GetString("texture"),
                        Material = roadStyleConfig.GetString("material"),
                        UvMap = textureMap[roadStyleConfig.GetInt("textureMap/@index")],
                    });
                }
                theme.RoadTypeStyleMapping.Add(typeName, styles);
            }
        }

        private Dictionary<int, RoadStyle.TextureUvMap> LoadRoadTextureMap(IConfigSection textureMapConfig)
        {
            var textureMaps = new Dictionary<int, RoadStyle.TextureUvMap>();
            foreach (var uvConfig in textureMapConfig.GetSections("roads/textureMap/uv"))
            {
                var index = uvConfig.GetInt("@index");
                textureMaps.Add(index, new RoadStyle.TextureUvMap()
                {
                    Main = GetUv(uvConfig.GetSection("main")).ToArray(),
                    Turn = GetUv(uvConfig.GetSection("turn")).ToArray(),
                });
            }
            return textureMaps;
        }

        #endregion

        private IEnumerable<Vector2> GetUv(IConfigSection uvsConfig)
        {
            foreach (var uvConfig in uvsConfig.GetSections("uv"))
            {
                yield return new Vector2(uvConfig.GetFloat("@x"), uvConfig.GetFloat("@y"));
            }
        }
    }
}
