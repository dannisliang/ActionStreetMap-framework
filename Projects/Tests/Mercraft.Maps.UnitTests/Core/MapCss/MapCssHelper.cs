using System.Collections.Generic;
using System.IO;
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

        public static Canvas GetCanvas()
        {
            return new Canvas();
        }

        public static Area GetArea(Dictionary<string, string> tags)
        {
            return new Area
            {
                Points = new[]
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
                Points = new[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 0)
                },
                Tags = tags
            };
        }
    }
}