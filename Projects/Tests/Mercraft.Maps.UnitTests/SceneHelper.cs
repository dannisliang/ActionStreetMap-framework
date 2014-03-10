using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mercraft.Core.Scene;

namespace Mercraft.Maps.UnitTests
{
    public static class SceneHelper
    {
        public static void DumpScene(MapScene mapScene)
        {
            using (var file = File.CreateText(@"f:\scene.txt"))
            {
                file.WriteLine("BUILDINGS:");
                var area = mapScene.Areas.ToList();
                for (int i = 0; i < area.Count; i++)
                {
                    var building = area[i];
                    file.WriteLine(@"\Areas {0}", (i + 1));
                    var lineOffset = "\t\t";
                    file.WriteLine("{0}Tags:", lineOffset);
                    foreach (var tag in building.Tags)
                    {
                        file.WriteLine("{0}\t{1}:{2}", lineOffset, tag.Key, tag.Value);
                    }
                    file.WriteLine("{0}Points:", lineOffset);
                    foreach (var point in building.Points)
                    {
                        file.WriteLine("{0}\tnew GeoCoordinate({1},{2}),", lineOffset, point.Latitude, point.Longitude);
                    }
                }
            }
        }
    }
}
