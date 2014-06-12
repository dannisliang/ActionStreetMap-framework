using System.IO;
using System.Linq;
using Mercraft.Core.Scene;

namespace Mercraft.Maps.UnitTests
{
    public static class SceneHelper
    {
        public static void DumpScene(MapScene mapScene)
        {
            using (var file = File.CreateText(@"f:\scene.txt"))
            {
                file.WriteLine("Areas:");
                var areas = mapScene.Areas.ToList();
                for (int i = 0; i < areas.Count; i++)
                {
                    var area = areas[i];
                    file.WriteLine(@"\tAreas {0}", area.Id);
                    var lineOffset = "\t\t";
                    file.WriteLine("{0}Tags:", lineOffset);
                    foreach (var tag in area.Tags)
                    {
                        file.WriteLine("{0}\t{1}:{2}", lineOffset, tag.Key, tag.Value);
                    }
                    file.WriteLine("{0}Points:", lineOffset);
                    foreach (var point in area.Points)
                    {
                        file.WriteLine("{0}\tnew GeoCoordinate({1},{2}),", lineOffset, point.Latitude, point.Longitude);
                    }
                }

                file.WriteLine("Ways:");
                var ways = mapScene.Ways.ToList();
                for (int i = 0; i < ways.Count; i++)
                {
                    var way = ways[i];
                    file.WriteLine(@"\tWays {0}", way.Id);
                    var lineOffset = "\t\t";
                    file.WriteLine("{0}Tags:", lineOffset);
                    foreach (var tag in way.Tags)
                    {
                        file.WriteLine("{0}\t{1}:{2}", lineOffset, tag.Key, tag.Value);
                    }
                    file.WriteLine("{0}Points:", lineOffset);
                    foreach (var point in way.Points)
                    {
                        file.WriteLine("{0}\tnew GeoCoordinate({1},{2}),", lineOffset, point.Latitude, point.Longitude);
                    }
                }
            }
        }
    }
}