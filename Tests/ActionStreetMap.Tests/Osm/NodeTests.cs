﻿using System.Linq;
using ActionStreetMap.Core;
using ActionStreetMap.Explorer.Infrastructure;
using ActionStreetMap.Infrastructure.Diagnostic;
using ActionStreetMap.Osm;
using ActionStreetMap.Osm.Data;
using ActionStreetMap.Osm.Visitors;
using ActionStreetMap.Core.Scene;
using NUnit.Framework;

namespace ActionStreetMap.Tests.Osm
{
    [TestFixture]
    public class NodeTests
    {
        [Test]
        public void CanLoadNodes()
        {
            // ARRANGE
            var dataSource = new PbfIndexListElementSource(TestHelper.TestBigPbfIndexListPath,
                TestHelper.GetFileSystemService(), new DefaultTrace());

            var bbox = BoundingBox.CreateBoundingBox(TestHelper.BerlinGeoCenter, 1000);

            var testModelVisitor = new TestModelVisitor();

            var elementManager = new ElementManager();

            // ACT
            elementManager.VisitBoundingBox(bbox, dataSource, new NodeVisitor(testModelVisitor, new ObjectPool()));

            // ASSERT
            Assert.AreEqual(4252, testModelVisitor.Nodes.Count());
        }
    }
}
