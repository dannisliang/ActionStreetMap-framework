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
                var buildings = mapScene.Buildings.ToList();
                for (int i = 0; i < buildings.Count; i++)
                {
                    var building = buildings[i];
                    file.WriteLine("\tBuilding {0}", (i + 1));
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
