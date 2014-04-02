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
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Algorithms
{
    [TestFixture]
    public class TriangulationTests
    {

        [Test]
        public void CanTriangulateNonStandard()
        {
            var verticies = new Vector2[]
            {
                new Vector2(669.0f, -181.5f),
                new Vector2(671.2f, -188.2f),
                new Vector2(682.9f, -184.4f),
                new Vector2(688.9f, -202.4f),
                new Vector2(670.0f, -208.6f),
                new Vector2(664.1f, -190.5f),
                new Vector2(671.2f, -188.2f),
            };
            var triangulator = new Triangulator(verticies);
            var triangles = triangulator.Triangulate();
        }

        [Test]
        public void CanTriangulateAreasAndWays()
        {
            using (Stream stream = new FileInfo(TestHelper.TestBigPbfFilePath).OpenRead())
            {
                var dataSource = new PbfElementSource(stream);

                var bbox = BoundingBox.CreateBoundingBox(TestHelper.BerlinGeoCenter, 1000);

                var scene = new MapScene();

                var elementManager = new ElementManager();

                elementManager.VisitBoundingBox(bbox, dataSource, new WayVisitor(scene));

                foreach (var area in scene.Areas)
                {
                    var verticies = PolygonHelper.GetVerticies2D(TestHelper.BerlinGeoCenter, area.Points.ToList());
                    var triangles = PolygonHelper.GetTriangles3D(verticies);
                }

                foreach (var way in scene.Ways)
                {
                    var verticies = PolygonHelper.GetVerticies2D(TestHelper.BerlinGeoCenter, way.Points.ToList());
                    var triangles = PolygonHelper.GetTriangles3D(verticies);
                }

                //SceneHelper.DumpScene(scene);
            }
        }
    }
}
