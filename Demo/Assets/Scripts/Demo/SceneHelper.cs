using System.IO;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Scene;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Visitors;

namespace Assets.Scripts.Demo
{
    public static class SceneHelper
    {
        public static void DumpScene(GeoCoordinate center, string filePath, float size)
        {
            using (Stream stream = new FileInfo(filePath).OpenRead())
            {
                var dataSource = new PbfElementSource(stream);
                var bbox = BoundingBox.CreateBoundingBox(center, size);
                var scene = new MapScene();
                var elementManager = new ElementManager();
                elementManager.VisitBoundingBox(bbox, dataSource, new WayVisitor(scene));

                DumpScene(scene);
            }

        }

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
