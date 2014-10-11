using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mercraft.Core.Utilities;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Formats.Json;
using Mercraft.Models.Buildings;
using Mercraft.Models.Buildings.Facades;
using Mercraft.Models.Buildings.Roofs;
using Mercraft.Models.Geometry;
using Mercraft.Models.Infos;
using Mercraft.Models.Roads;
using UnityEngine;

using Rect = Mercraft.Models.Geometry.Rect;

namespace Mercraft.Explorer.Themes
{
    public interface IThemeProvider
    {
        Theme Get();
    }

    public class ThemeProvider : IThemeProvider, IConfigurable
    {
        private readonly IPathResolver _pathResolver;
        private const string BuildingsThemeFile = @"buildings";
        private const string RoadsThemeFile = @"roads";
        private const string InfosThemeFile = @"infos";

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
            var infoStyleProvider = GetInfoStyleProvider(configSection);
            _theme = new Theme(buildingStyleProvider, roadStyleProvider, infoStyleProvider);
        }

        #region Buildings

        public IBuildingStyleProvider GetBuildingStyleProvider(IConfigSection configSection)
        {
            var facadeStyleMapping = new Dictionary<string, List<BuildingStyle.FacadeStyle>>();
            var roofStyleMapping = new Dictionary<string, List<BuildingStyle.RoofStyle>>();
            foreach (var buildThemeConfig in configSection.GetSections(BuildingsThemeFile))
            {
                var path = buildThemeConfig.GetString("path");
                using (var reader = new StreamReader(_pathResolver.Resolve(path)))
                {
                    var jsonStr = reader.ReadToEnd();
                    var json = JSON.Parse(jsonStr);

                    var facadeStyles = GetFacadeStyles(json);
                    var roofStyles = GetRoofStyles(json);

                    var types = json["name"].AsArray.Childs.Select(t => t.Value);
                    foreach (var type in types)
                    {
                        facadeStyleMapping.Add(type, facadeStyles);
                        roofStyleMapping.Add(type, roofStyles);
                    }
                }
            }
            return new BuildingStyleProvider(facadeStyleMapping, roofStyleMapping);
        }

        private List<BuildingStyle.FacadeStyle> GetFacadeStyles(JSONNode json)
        {
            var facadeStyles = new List<BuildingStyle.FacadeStyle>();
            foreach (JSONNode node in json["facades"].AsArray)
            {
                var builders = node["builders"].AsArray.Childs
                    .Select(t => _facadeBuilders.Single(b => b.Name == t.Value)).ToArray();
                var path = node["path"].Value;
                var size = new Size(node["size"]["width"].AsInt, node["size"]["height"].AsInt);
                foreach (JSONNode textureNode in node["textures"].AsArray)
                {
                    var map = textureNode["map"];
                    facadeStyles.Add(new BuildingStyle.FacadeStyle()
                    {
                        Height = textureNode["height"].AsInt,
                        Width = textureNode["width"].AsInt,
                        Material = textureNode["material"].Value,
                        Color = ColorUtility.FromUnknown(textureNode["color"].Value),
                        Builders = builders,
                        Path = path,
                        FrontUvMap = GetUvMap(map["front"], size),
                        BackUvMap = GetUvMap(map["back"], size),
                        SideUvMap = GetUvMap(map["side"], size)
                    });
                }
            }
            return facadeStyles;
        }

        private List<BuildingStyle.RoofStyle> GetRoofStyles(JSONNode json)
        {
            var roofStyles = new List<BuildingStyle.RoofStyle>();
            foreach (JSONNode node in json["roofs"].AsArray)
            {
                var builders = node["builders"].AsArray.Childs
                    .Select(t => _roofBuilders.Single(b => b.Name == t.Value)).ToArray();
                var path = node["path"].Value;
                var size = new Size(node["size"]["width"].AsInt, node["size"]["height"].AsInt);
                foreach (JSONNode textureNode in node["textures"].AsArray)
                {
                    var map = textureNode["map"];
                    roofStyles.Add(new BuildingStyle.RoofStyle()
                    {
                        Type = textureNode["type"],
                        Height = textureNode["height"].AsInt,
                        Material = textureNode["material"].Value,
                        Color = ColorUtility.FromUnknown(textureNode["color"].Value),
                        Builders = builders,
                        Path = path,
                        FrontUvMap = GetUvMap(map["front"], size),
                        SideUvMap = GetUvMap(map["side"], size),
                    });
                }
            }

            return roofStyles;
        }

        #endregion

        #region Roads

        public IRoadStyleProvider GetRoadStyleProvider(IConfigSection configSection)
        {
            var roadTypeStyleMapping = new Dictionary<string, List<RoadStyle>>();
            foreach (var roadThemeConfig in configSection.GetSections(RoadsThemeFile))
            {
                var path = roadThemeConfig.GetString("path");
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
            var roadStyles = new List<RoadStyle>();
            foreach (JSONNode node in json["roads"].AsArray)
            {
                var path = node["path"].Value;
                var size = new Size(node["size"]["width"].AsInt, node["size"]["height"].AsInt);
                foreach (JSONNode textureNode in node["textures"].AsArray)
                {
                    var map = textureNode["map"];
                    roadStyles.Add(new RoadStyle()
                    {
                        Height = textureNode["height"].AsInt,
                        Material = textureNode["material"].Value,
                        Color = ColorUtility.FromUnknown(textureNode["color"].Value),
                        
                        Path = path,
                        MainUvMap = GetUvMap(map["main"], size),
                        TurnUvMap = GetUvMap(map["turn"], size),
                    });
                }
            }

            return roadStyles;
        }

        #endregion

        private IInfoStyleProvider GetInfoStyleProvider(IConfigSection configSection)
        {
            // NOTE ignore name of style pack - just use one collection
            var infoStyleMap = new Dictionary<string, InfoStyle>();
            foreach (var infoThemeConfig in configSection.GetSections(InfosThemeFile))
            {
                var path = infoThemeConfig.GetString("path");
                using (var reader = new StreamReader(_pathResolver.Resolve(path)))
                {
                    var jsonStr = reader.ReadToEnd();
                    var json = JSON.Parse(jsonStr);
                    FillInfoStyleList(json, infoStyleMap);
                }
            }
            return new InfoStyleProvider(infoStyleMap);
        }

        private void FillInfoStyleList(JSONNode json, Dictionary<string, InfoStyle> infoStyleMap)
        {
            foreach (JSONNode node in json["infos"].AsArray)
            {
                var path = node["path"].Value;
                var size = new Size(node["size"]["width"].AsInt, node["size"]["height"].AsInt);
                foreach (JSONNode textureNode in node["textures"].AsArray)
                {
                    var map = textureNode["map"];
                    infoStyleMap.Add(textureNode["key"].Value, new InfoStyle()
                    {
                        Path = path,
                        UvMap = GetUvMap(map["main"], size),
                    });
                }
            }
        }

        private Rect GetUvMap(string value, Size size)
        {
            // expect x,y,width,height and (0,0) is left bottom corner
            if (value == null)
                return null;

            var values = value.Split(',');
            if (values.Length != 4)
                throw new InvalidOperationException(String.Format(ErrorStrings.InvalidUvMappingDefinition, value));

            var width = (float)int.Parse(values[2]);
            var height = (float)int.Parse(values[3]);

            var offset = int.Parse(values[1]);
            var x = (float)int.Parse(values[0]);
            var y = Math.Abs( (offset + height) - size.Height);

            var leftBottom = new Vector2(x / size.Width, y / size.Height);
            var rightUpper = new Vector2((x + width) / size.Width, (y + height) / size.Height);

            return new Rect(leftBottom, rightUpper);
        }
    }
}
