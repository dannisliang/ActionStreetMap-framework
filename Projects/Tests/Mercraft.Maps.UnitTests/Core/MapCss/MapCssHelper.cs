using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Mercraft.Core;
using Mercraft.Core.MapCss;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;

namespace Mercraft.Maps.UnitTests.Core.MapCss
{
    public static class MapCssHelper
    {
        public static Stylesheet GetStylesheet(string content)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;
            var provider = new StylesheetProvider(stream);
            return provider.Get();
        }

        public static IList<Style> GetStyles(Stylesheet stylesheet)
        {
            // NOTE don't want to expose these fieds as public API, so use reflection in tests
            Func<string, IList<Style>> getter = name =>
            {
                var sField = typeof(Stylesheet).GetField("_styles", BindingFlags.NonPublic |
                    BindingFlags.Instance | BindingFlags.GetField);
                var sFieldValue = (sField.GetValue(stylesheet) as StyleCollection);

                var field = typeof(StyleCollection).GetField(name, BindingFlags.NonPublic |
                   BindingFlags.Instance | BindingFlags.GetField);
                return (field.GetValue(sFieldValue) as List<Style>);
            };

            var result = new List<Style>();
            result.AddRange(getter("_canvasStyles"));
            result.AddRange(getter("_areaStyles"));
            result.AddRange(getter("_wayStyles"));
            result.AddRange(getter("_nodeStyles"));

            return result;
        }

        public static Canvas GetCanvas()
        {
            return new Canvas();
        }

        public static Area GetArea(Dictionary<string, string> tags)
        {
            return new Area
            {
                Points = new List<GeoCoordinate>()
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 0)
                },
                Tags = tags
            };
        }

        public static Way GetWay(Dictionary<string, string> tags)
        {
            return new Way
            {
                Points = new List<GeoCoordinate>()
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 0)
                },
                Tags = tags
            };
        }

        public static Node GetNode(Dictionary<string, string> tags)
        {
            return new Node
            {
                Point = new GeoCoordinate(0, 0),
                Tags = tags
            };
        }
    }
}