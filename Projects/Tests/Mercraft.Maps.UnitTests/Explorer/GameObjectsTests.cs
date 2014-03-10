using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.Scene;
using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Visitors;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Explorer
{
    [TestFixture]
    public class GameObjectsTests
    {
        [Test(Description = "Emulate polygone mesh building logic")]
        public void CanTriangulateBuildingPolygons()
        {
            var center = TestHelper.BerlinGeoCenter;
            using (Stream stream = new FileInfo(TestHelper.TestBigPbfFilePath).OpenRead())
            {
                var elementSource = new PbfElementSource(stream);
                var bbox = BoundingBox.CreateBoundingBox(center, 1000);

                var scene = new MapScene();
                var visitor = new CompositeVisitor(new List<IElementVisitor>
                {
                    new AreaVisitor(scene)
                });

                var elementManager = new ElementManager();
                elementManager.VisitBoundingBox(bbox, elementSource, visitor);

                var buildings = scene.Areas.Where(a => a.Tags.Any(t => t.Key.Contains("building"))).
                    Where(a => a.Points.Count > 2) // filter some specific ways with building tag
                    .ToList();

                int i = 1;
                foreach (var building in buildings)
                {
                    var verticies2D = PolygonHelper.GetVerticies2D(center, building.Points.ToList());
                    PolygonHelper.GetVerticies3D(verticies2D, 1, 10);
                    PolygonHelper.GetUV(verticies2D);
                    PolygonHelper.GetTriangles(verticies2D);
                    if(i % 10 == 0)
                        Console.WriteLine(i + "\n");
                    i++;
                }
            }
        }

      
    }
}
