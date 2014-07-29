using System.Collections.Generic;
using System.Linq;
using Mercraft.Infrastructure.Config;
using Mercraft.Models.Buildings;
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
        private const string ThemesKey = "theme";
        
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
            _themes = new Dictionary<string, Theme>();
            foreach (var themeConfig in configSection.GetSections(ThemesKey))
            {
                var theme = new Theme();
                theme.BuildingTypeStyleMapping = new Dictionary<string, List<BuildingStyle>>();
                theme.Name = themeConfig.GetString("@name");

                // set default theme name
                if (string.IsNullOrEmpty(_defaultThemeName))
                    _defaultThemeName = theme.Name;

                ConfigureBuildings(themeConfig, theme);

                _themes.Add(theme.Name, theme);
            }
        }

        private void ConfigureBuildings(IConfigSection themeConfig, Theme theme)
        {
            var textureMap = LoadTextureMap(themeConfig);
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
                        TextureMap = textureMap[buildingStyleConfig.GetInt("textureMap/@index")]
                    });
                }
                theme.BuildingTypeStyleMapping.Add(typeName, styles);
            }
        }

        private Dictionary<int, BuildingTextureMap> LoadTextureMap(IConfigSection textureMapConfig)
        {
            var textureMaps = new Dictionary<int, BuildingTextureMap>();
            foreach (var uvConfig in textureMapConfig.GetSections("buildings/textureMap/uv"))
            {
                var index = uvConfig.GetInt("@index");
                textureMaps.Add(index, new BuildingTextureMap()
                {
                    FrontUv = GetUv(uvConfig.GetSection("facade/front")).ToArray(),
                    BackUv = GetUv(uvConfig.GetSection("facade/back")).ToArray(),
                    SideUv = GetUv(uvConfig.GetSection("facade/side")).ToArray(),
                    RoofUv = GetUv(uvConfig.GetSection("roof")).ToArray(),
                });
            }
            return textureMaps;
        }

        private IEnumerable<Vector2> GetUv(IConfigSection uvsConfig)
        {
            foreach (var uvConfig in uvsConfig.GetSections("uv"))
            {
                yield return new Vector2(uvConfig.GetFloat("@x"), uvConfig.GetFloat("@y"));
            }
        }
    }
}
