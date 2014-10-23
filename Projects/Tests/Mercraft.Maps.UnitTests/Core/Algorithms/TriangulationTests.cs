using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Explorer.Infrastructure;
using Mercraft.Infrastructure.Diagnostic;
using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Visitors;
using Mercraft.Maps.UnitTests.Osm;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Core.Algorithms
{
    /// <summary>
    ///     These tests test functionality which seems to be depricated in near future (?)
    /// </summary>
    [TestFixture]
    public class TriangulationTests
    {
        [Test]
        public void CanTriangulateNonStandard()
        {
            // ARRANGE
            var verticies = new List<MapPoint>()
            {
                new MapPoint(669.0f, -181.5f),
                new MapPoint(671.2f, -188.2f),
                new MapPoint(682.9f, -184.4f),
                new MapPoint(688.9f, -202.4f),
                new MapPoint(670.0f, -208.6f),
                new MapPoint(664.1f, -190.5f),
                new MapPoint(671.2f, -188.2f)
            };

            // ACT & ASSERT
            Triangulator.Triangulate(verticies);
        }

        [Test]
        public void CanTriangulateAreasAndWays()
        {
            // ARRANGE
            var dataSource = new PbfIndexListElementSource(TestHelper.TestBigPbfIndexListPath,
                TestHelper.GetFileSystemService(), new DefaultTrace());

            var bbox = BoundingBox.CreateBoundingBox(TestHelper.BerlinGeoCenter, 1000);

            var scene = new TestModelVisitor();

            var elementManager = new ElementManager();

            elementManager.VisitBoundingBox(bbox, dataSource, new WayVisitor(scene, new ObjectPool()));

            // ACT & ARRANGE
            foreach (var area in scene.Areas)
            {
                var verticies = new List<MapPoint>();
                PointHelper.GetVerticies2D(TestHelper.BerlinGeoCenter, area.Points, verticies);
                PointHelper.GetTriangles3D(verticies);
            }

            foreach (var way in scene.Ways)
            {
                var verticies = new List<MapPoint>();
                PointHelper.GetVerticies2D(TestHelper.BerlinGeoCenter, way.Points, verticies);
                var triangles = PointHelper.GetTriangles3D(verticies);
            }
        }
    }
}