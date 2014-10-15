using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Visitors;
using Moq;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Osm
{
    [TestFixture]
    public class ElementManagerTests
    {
        [Test(Description = "Tests whether we can process way which crosses tile border once (should be loaded for two tiles)")]
        public void CanProcessWayCrossingBorderOnce()
        {
            // TODO think how to distinguesh area (building) from way (road)

            // ARRANGE
            var elementManager = new ElementManager();

            # region Center Bbox elements
            var centerCoordinate = new GeoCoordinate(52, 13);
            var boundingBoxCenter = BoundingBox.CreateBoundingBox(centerCoordinate, 50);          
            var node1 = new Node()
            {
                Id = 1,
                Coordinate = GetCoordinateInBbox(boundingBoxCenter, 0.0001f, 0.0001f)
            };
            var node2 = new Node()
            {
                Id = 2,
                Coordinate = GetCoordinateInBbox(boundingBoxCenter, 0.0002f, 0.0002f)
            };
            var node3 = new Node()
            {
                Id = 3,
                Coordinate = GetCoordinateInBbox(boundingBoxCenter, 0.0003f, 0.0003f)
            };
            // this way should be processed for CENTER tile only
            var controlWay1 = new Way()
            {
                Id = 6,
                NodeIds = new List<long>()
                {
                    1, 2
                }
            };
            #endregion

            #region Left Bbox elements
            // left tile
            var leftCoordinate = GeoProjection.ToGeoCoordinate(centerCoordinate, new MapPoint(-50, 0));
            var boundingBoxLeft = BoundingBox.CreateBoundingBox(leftCoordinate, 50); 
            var node4 = new Node()
            {
                Id = 4,
                Coordinate = GetCoordinateInBbox(boundingBoxLeft, 0.0004f, 0.0004f),
            };
            var node5 = new Node()
            {
                Id = 5,
                Coordinate = GetCoordinateInBbox(boundingBoxLeft, 0.0005f, 0.0005f),
            };
            var node6 = new Node()
            {
                Id = 6,
                Coordinate = GetCoordinateInBbox(boundingBoxLeft, 0.0006f, 0.0006f)
            };
            // this way should be processed for LEFT tile only
            var controlWay2 = new Way()
            {
                Id = 7,
                NodeIds = new List<long>()
                {
                    4, 5, 6
                }
            };

            #endregion

            // this way should be processed for LEFT and CENTER tiles
            var testWay = new Way()
            {
                Id = 0,
                NodeIds = new List<long>()
                {
                    1, 2, 3, 4, 5
                }
            };

            var elementSource = new Mock<IElementSource>();
            elementSource.Setup(s => s.Get(boundingBoxCenter)).Returns(new List<Element>()
            {
                node1, node2, node3, node4, node5, testWay, controlWay1
            });
            elementSource.Setup(s => s.Get(boundingBoxLeft)).Returns(new List<Element>()
            {
                Clone(node4), Clone(node5), node6, controlWay2
            });

            var nodeList = new List<Node>()
            {
                node1, node2, node3, node4, node5, node6
            };

            List<Node> list = nodeList;
            elementSource.Setup(s => s.GetNode(It.IsAny<long>()))
                .Returns<long>(l => list.Single(n => n.Id == l));

            var elementVisitor = new Mock<IElementVisitor>();
            var visitedWays = new List<Way>();
            elementVisitor.Setup(v => v.VisitWay(It.IsAny<Way>()))
                .Callback<Way>(visitedWays.Add);

            node4.IsOutOfBox = true;
            node5.IsOutOfBox = true;

            // ACT & ASSERT

            // first tile
            elementManager.VisitBoundingBox(boundingBoxCenter, elementSource.Object, elementVisitor.Object);
            Assert.AreEqual(2, visitedWays.Count);
            Assert.IsTrue(visitedWays.Any(w => w.Id == 0));
            Assert.IsTrue(visitedWays.Any(w => w.Id == 6));

            // NOTE we assume that element source should return always new nodes for different requests
            // that's why we need to clone node4 and node5 which should be memorized by ElementManager
            // after first request. If we reuse old ones, it will break the underlying algorithm
            nodeList = new List<Node>()
            {
                node1, node2, node3, Clone(node4),  Clone(node5), node6
            };

            visitedWays.Clear();
            // second tile
            elementManager.VisitBoundingBox(boundingBoxLeft, elementSource.Object, elementVisitor.Object);
            Assert.AreEqual(2, visitedWays.Count);
            Assert.IsTrue(visitedWays.Any(w => w.Id == 0), "Unable to load way for second tile");
            Assert.IsTrue(visitedWays.Any(w => w.Id == 7));
        }

        private Node Clone(Node node)
        {
            return new Node()
            {
                Id = node.Id,
                Coordinate = node.Coordinate
            };
        }

        private GeoCoordinate GetCoordinateInBbox(BoundingBox bbox, float xOffset, float yOffset)
        {
            var geoCoordinate = new GeoCoordinate(bbox.MaxPoint.Latitude - xOffset,
                bbox.MaxPoint.Longitude - yOffset);
            Assert.IsTrue(bbox.Contains(geoCoordinate));

            return geoCoordinate;
        }
    }
}
