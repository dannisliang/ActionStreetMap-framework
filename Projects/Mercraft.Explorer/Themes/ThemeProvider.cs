using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
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
        private readonly IEnumerable<IFacadeBuilder> _facadeBuilders;
        private readonly IEnumerable<IRoofBuilder> _roofBuilders;
        private string _defaultThemeName;
        private const string ThemesKey = "themes/theme";
        private Dictionary<string, Theme> _themes;

        [Dependency]
        public ThemeProvider(IEnumerable<IFacadeBuilder> facadeBuilders, 
            IEnumerable<IRoofBuilder> roofBuilders)
        {
            _facadeBuilders = facadeBuilders.ToArray();
            _roofBuilders = roofBuilders.ToArray();
        }

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
            _themes = new Dictionary<string, Theme>();
            foreach (var themeConfig in configSection.GetSections(ThemesKey))
            {
                var buildingStyleProvider = GetBuildingStyleProvider(themeConfig);
                var roadStyleProvider = GetRoadStyleProvider(themeConfig);

                var theme = new Theme(buildingStyleProvider, roadStyleProvider);
                theme.Name = themeConfig.GetString("@name");
                
                // set default theme name
                if (string.IsNullOrEmpty(_defaultThemeName))
                    _defaultThemeName = theme.Name;

                _themes.Add(theme.Name, theme);
            }
        }

        #region Buildings

        private IBuildingStyleProvider GetBuildingStyleProvider(IConfigSection themeConfig)
        {
            var textureMap = LoadTextureMap("buildings/textureMap/uv", themeConfig);
            var buildingTypeStyleMapping = new Dictionary<string, List<BuildingStyle>>();
            foreach (var buildingTypeConfig in themeConfig.GetSections("buildings/types/type"))
            {
                var typeName = buildingTypeConfig.GetString("@name");
                var styles = new List<BuildingStyle>();
                foreach (var buildingStyleConfig in buildingTypeConfig.GetSections("style"))
                {
                    styles.Add(new BuildingStyle()
                    {
                        Facade = GetFacadeStyle(textureMap, buildingStyleConfig.GetSection("facade")),
                        Roof = GetRoofStyle(textureMap, buildingStyleConfig.GetSection("roof")),
                        Floors = buildingStyleConfig.GetInt("floors/@size")
                    });
                }
                buildingTypeStyleMapping.Add(typeName, styles);
            }

            return new BuildingStyleProvider(buildingTypeStyleMapping);
        }

        private BuildingStyle.RoofStyle GetRoofStyle(Dictionary<int, Vector2[]>  textureMap, IConfigSection roofConfig)
        {
            return new BuildingStyle.RoofStyle()
            {
                Texture = roofConfig.GetString("texture"),
                Material = roofConfig.GetString("material"),
                Builder = _roofBuilders.Single(b => b.Name == roofConfig.GetString("builder/@name")),
                UvMap = textureMap[roofConfig.GetInt("uvMap/main/@index")]
            };
        }

        private BuildingStyle.FacadeStyle GetFacadeStyle(Dictionary<int, Vector2[]> textureMap, IConfigSection facadeConfig)
        {
            return new BuildingStyle.FacadeStyle()
            {
                Texture = facadeConfig.GetString("texture"),
                Material = facadeConfig.GetString("material"),
                Builder = _facadeBuilders.Single(b => b.Name == facadeConfig.GetString("builder/@name")),
                FrontUvMap = textureMap[facadeConfig.GetInt("uvMap/front/@index")],
                BackUvMap = textureMap[facadeConfig.GetInt("uvMap/back/@index")],
                SideUvMap = textureMap[facadeConfig.GetInt("uvMap/side/@index")],
            };
        }

        #endregion

        #region Roads

        private IRoadStyleProvider GetRoadStyleProvider(IConfigSection themeConfig)
        {
            var roadTypeStyleMapping = new Dictionary<string, List<RoadStyle>>();
            var textureMap = LoadTextureMap("roads/textureMap/uv", themeConfig);
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
                        UvMap = new RoadStyle.TextureUvMap()
                        {
                            Main = textureMap[roadStyleConfig.GetInt("uvMap/main/@index")],
                            Turn = textureMap[roadStyleConfig.GetInt("uvMap/turn/@index")],
                        }
                    });
                }
                roadTypeStyleMapping.Add(typeName, styles);
            }

            return new RoadStyleProvider(roadTypeStyleMapping);
        }

        #endregion

        private Dictionary<int, Vector2[]> LoadTextureMap(string path, IConfigSection textureMapConfig)
        {
            var textureMaps = new Dictionary<int, Vector2[]>();
            foreach (var uvConfig in textureMapConfig.GetSections(path))
            {
                var index = uvConfig.GetInt("@index");
                textureMaps.Add(index, GetUv(uvConfig).ToArray());
            }
            return textureMaps;
        }

        private IEnumerable<Vector2> GetUv(IConfigSection uvsConfig)
        {
            foreach (var uvConfig in uvsConfig.GetSections("v"))
            {
                yield return new Vector2(uvConfig.GetFloat("@x"), uvConfig.GetFloat("@y"));
            }
        }
    }
}
