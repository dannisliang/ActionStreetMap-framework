using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Utilities;
using ActionStreetMap.Osm;
using ActionStreetMap.Osm.Data;
using ActionStreetMap.Osm.Entities;
using ActionStreetMap.Osm.Visitors;
using ActionStreetMap.Tests.Osm.Stubs;
using Moq;
using NUnit.Framework;

namespace ActionStreetMap.Tests.Osm
{
    [TestFixture]
    public class ElementManagerTests
    {
        #region Tests which are using mocks
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
        #endregion

        #region Test which are using pbf source

        // test file contains:
        // way=100 (1,2,3), way=101 (3,4,5), way=102 (5,6,7), way=103 (8,9,10,11,1)
        // relation includes all ways as outer
        //
        // right center. Split by middle point divides to 1, 2, 3, 9, 10, 11 (+12) node ids
        // affected way ids: 100, 101, 103
        [TestCase(53.02477692964, 27.56647518709, 53.02460731988, 27.59057969346)]
        // left point. Split by middle point divides to  4, 5, 6, 7, 8 (+14) node ids
        // affected way ids: 101, 102, 103
        [TestCase(53.02477692964, 27.56647518709, 53.02470575919, 27.54510491297)]
        public void CanLoadFullRelationAcrossTile(double middlePointLat, double middlePointLon, double centerPointLat, double centerPointLon)
        {
            // ARRANGE
            var elementManager = new ElementManager();
            var middlePoint = new GeoCoordinate(middlePointLat, middlePointLon);
            var testDataCenter = new GeoCoordinate(centerPointLat, centerPointLon);
            var tileSize = GeoCoordinateHelper.CalcDistance(testDataCenter, middlePoint);
            var boundingBox = BoundingBox.CreateBoundingBox(testDataCenter, tileSize);
            using (var pbfDataSource = new PbfElementSource(new FileStream(TestHelper.TestMulitplyOuterWaysInRelationPbf, FileMode.Open)))
            {
                var elementVisitor = new CountableElementVisitor();

                // ACT
                elementManager.VisitBoundingBox(boundingBox, pbfDataSource, elementVisitor);

                // ASSERT
                var relation = elementVisitor.Elements.SingleOrDefault(e => e is Relation) as Relation;
                Assert.IsNotNull(relation, "Cannot find relation");
                Assert.AreEqual(4, relation.Members.Count);
                Assert.IsTrue(relation.Members.Distinct().Count() == 4);
            }
        }

        #endregion
    }
}
