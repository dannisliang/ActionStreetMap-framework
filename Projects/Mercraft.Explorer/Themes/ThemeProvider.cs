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
            var textureMap = LoadBuildingTextureMap(themeConfig);
            var buildingTypeStyleMapping = new Dictionary<string, List<BuildingStyle>>();
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
                        FacadeBuilder = _facadeBuilders.Single(b => b.Name == buildingStyleConfig.GetString("facade/@builder")),
                        RoofBuilder = _roofBuilders.Single(b => b.Name == buildingStyleConfig.GetString("roof/@builder"))
                    });
                }
                buildingTypeStyleMapping.Add(typeName, styles);
            }

            return new BuildingStyleProvider(buildingTypeStyleMapping);
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

        private IRoadStyleProvider GetRoadStyleProvider(IConfigSection themeConfig)
        {
            var roadTypeStyleMapping = new Dictionary<string, List<RoadStyle>>();
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
                roadTypeStyleMapping.Add(typeName, styles);
            }

            return new RoadStyleProvider(roadTypeStyleMapping);
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
