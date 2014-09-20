using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Formats.Json;
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
    }

    public class ThemeProvider : IThemeProvider, IConfigurable
    {
        private readonly IPathResolver _pathResolver;
        private const string BuildingsThemeFile = @"buildings/include";
        private const string RoadsThemeFile = @"roads/include";

        private readonly IEnumerable<IFacadeBuilder> _facadeBuilders;
        private readonly IEnumerable<IRoofBuilder> _roofBuilders;

        private Theme _theme;


        [Dependency]
        public ThemeProvider(IPathResolver pathResolver,
            IEnumerable<IFacadeBuilder> facadeBuilders,
            IEnumerable<IRoofBuilder> roofBuilders)
        {
            _pathResolver = pathResolver;
            _facadeBuilders = facadeBuilders.ToArray();
            _roofBuilders = roofBuilders.ToArray();
        }

        public Theme Get()
        {
            return _theme;
        }

        public void Configure(IConfigSection configSection)
        {
            var buildingStyleProvider = GetBuildingStyleProvider(configSection);
            var roadStyleProvider = GetRoadStyleProvider(configSection);
            _theme = new Theme(buildingStyleProvider, roadStyleProvider);
        }

        #region Buildings

        public IBuildingStyleProvider GetBuildingStyleProvider(IConfigSection configSection)
        {
            var buildingTypeStyleMapping = new Dictionary<string, List<BuildingStyle>>();
            foreach (var buildThemeConfig in configSection.GetSections(BuildingsThemeFile))
            {
                var path = buildThemeConfig.GetString("@path");
                using (var reader = new StreamReader(_pathResolver.Resolve(path)))
                {
                    var jsonStr = reader.ReadToEnd();
                    var json = JSON.Parse(jsonStr);

                    var buildingStyles = GetBuildingStyles(json);

                    var types = json["name"].AsArray.Childs.Select(t => t.Value);
                    foreach (var type in types)
                        buildingTypeStyleMapping.Add(type, buildingStyles);
                }
            }
            return new BuildingStyleProvider(buildingTypeStyleMapping);
        }

        private List<BuildingStyle> GetBuildingStyles(JSONNode json)
        {
            var buildingStyles = new List<BuildingStyle>();
            var uvs = LoadTextureMap(json);
            foreach (JSONNode entry in json["entries"].AsArray)
            {
                buildingStyles.Add(new BuildingStyle()
                {
                    Facade = GetFacadeStyle(uvs, entry["facade"]),
                    Roof = GetRoofStyle(uvs, entry["roof"]),
                });
            }
            return buildingStyles;
        }

        private BuildingStyle.FacadeStyle GetFacadeStyle(Dictionary<int, Vector2[]> textureMap, JSONNode node)
        {
            var desc = node["desc"];
            var render = node["render"];
            return new BuildingStyle.FacadeStyle()
            {
                Floors = desc["floors"].AsInt,
                Textures = render["textures"].AsArray.Childs.Select(t => t.Value).ToArray(),
                Materials = render["materials"].AsArray.Childs.Select(t => t.Value).ToArray(),
                Builders = render["builders"].AsArray.Childs.Select(t => _facadeBuilders.Single(b => b.Name ==t.Value)).ToArray(),
                FrontUvMap = textureMap[render["uvs"]["front"].AsInt],
                BackUvMap = textureMap[render["uvs"]["back"].AsInt],
                SideUvMap = textureMap[render["uvs"]["side"].AsInt],
            };
        }

        private BuildingStyle.RoofStyle GetRoofStyle(Dictionary<int, Vector2[]> textureMap, JSONNode node)
        {
            var render = node["render"];
            return new BuildingStyle.RoofStyle()
            {
                Textures = render["textures"].AsArray.Childs.Select(t => t.Value).ToArray(),
                Materials = render["materials"].AsArray.Childs.Select(t => t.Value).ToArray(),
                Builders = render["builders"].AsArray.Childs.Select(t => _roofBuilders.Single(b => b.Name == t.Value)).ToArray(),
                UvMap = textureMap[render["uvs"]["main"].AsInt]
            };
        }

        #endregion

        #region Roads

        public IRoadStyleProvider GetRoadStyleProvider(IConfigSection configSection)
        {
            var roadTypeStyleMapping = new Dictionary<string, List<RoadStyle>>();
            foreach (var roadThemeConfig in configSection.GetSections(RoadsThemeFile))
            {
                var path = roadThemeConfig.GetString("@path");
                using (var reader = new StreamReader(_pathResolver.Resolve(path)))
                {
                    var jsonStr = reader.ReadToEnd();
                    var json = JSON.Parse(jsonStr);
                    var roadStyles = GetRoadStyles(json);

                    var types = json["name"].AsArray.Childs.Select(t => t.Value);
                    foreach (var type in types)
                        roadTypeStyleMapping.Add(type, roadStyles);
                }
            }
            return new RoadStyleProvider(roadTypeStyleMapping);
        }

        private List<RoadStyle> GetRoadStyles(JSONNode json)
        {
            var buildingStyles = new List<RoadStyle>();
            var uvs = LoadTextureMap(json);
            foreach (JSONNode entry in json["entries"].AsArray)
            {
                buildingStyles.Add(new RoadStyle()
                {
                    Textures = entry["textures"].AsArray.Childs.Select(t => t.Value).ToArray(),
                    Materials = entry["materials"].AsArray.Childs.Select(t => t.Value).ToArray(),
                    UvMap = new RoadStyle.TextureUvMap()
                    {
                        Main = uvs[entry["uvs"]["main"].AsInt],
                        Turn = uvs[entry["uvs"]["turn"].AsInt]
                    }
                });
            }
            return buildingStyles;
        }

        #endregion

        private Dictionary<int, Vector2[]> LoadTextureMap(JSONNode json)
        {
            var textureMaps = new Dictionary<int, Vector2[]>();
            foreach (JSONNode uvConfig in json["uvs"].AsArray)
            {
                var index = uvConfig["index"].AsInt;
                textureMaps.Add(index, GetUv(uvConfig).ToArray());
            }
            return textureMaps;
        }

        private IEnumerable<Vector2> GetUv(JSONNode uvsConfig)
        {
            foreach (JSONNode uvConfig in uvsConfig["data"].AsArray)
                yield return new Vector2(uvConfig["x"].AsFloat, uvConfig["y"].AsFloat);
        }
    }
}
