﻿using System.Collections.Generic;
using System.Linq;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Scene.World.Roads;
using NUnit.Framework;

namespace ActionStreetMap.Tests.Core.World
{
    [TestFixture]
    public class RoadTests
    {
        [Test]
        public void CanComposeThreeRoadElementsTogether()
        {
            // ARRANGE
            var roadElements = new List<RoadElement>()
            {
                new RoadElement()
                {
                    Points = new List<MapPoint>()
                    {
                        new MapPoint(13.1f, 52.1f), 
                        new MapPoint(13.2f, 52.1f),
                        new MapPoint(13.3f, 52.1f),
                        new MapPoint(13.4f, 52.1f),
                        new MapPoint(13.5f, 52.1f),
                    }
                },
                new RoadElement()
                {
                    Points = new List<MapPoint>()
                    {
                        new MapPoint(13.5f, 52.1f),
                        new MapPoint(13.2f, 52.2f),
                        new MapPoint(13.3f, 52.2f),
                        new MapPoint(13.4f, 52.2f),
                        new MapPoint(13.5f, 52.2f),
                    }
                },
                new RoadElement()
                {
                    Points = new List<MapPoint>()
                    {
                        new MapPoint(13.5f, 52.2f),
                        new MapPoint(13.2f, 52.3f),
                        new MapPoint(13.3f, 52.3f),
                        new MapPoint(13.4f, 52.3f),
                        new MapPoint(13.5f, 52.3f),
                    }
                }
            };

            // ACT
            var roads = RoadElementComposer.Compose(roadElements).ToArray();

            // ASSERT
            Assert.AreEqual(1, roads.Length);
            Assert.AreEqual(3, roads[0].Count);
        }

        [Test]
        public void CanComposeSeveral()
        {
             // ARRANGE
            var roadElements = new List<RoadElement>()
            {
                #region Should be joined
                new RoadElement()
                {
                    Points = new List<MapPoint>()
                    {
                        new MapPoint(13.1f, 52.1f),
                        new MapPoint(13.5f, 52.1f),
                    }
                },
                new RoadElement()
                {
                    Points = new List<MapPoint>()
                    {
                        new MapPoint(13.5f, 52.1f),
                        new MapPoint(13.5f, 52.2f),
                    }
                },
                #endregion
                #region Random one
                 new RoadElement()
                {
                    Points = new List<MapPoint>()
                    {
                        new MapPoint(13.1f, 52.1f),
                        new MapPoint(13.5f, 52.1f),
                    }
                },
                #endregion

                #region Should be joined
                new RoadElement()
                {
                    Points = new List<MapPoint>()
                    {
                        new MapPoint(14.1f, 52.1f),
                        new MapPoint(14.5f, 52.1f),
                    }
                },
                new RoadElement()
                {
                    Points = new List<MapPoint>()
                    {
                        new MapPoint(14.5f, 52.1f),
                        new MapPoint(14.5f, 52.2f),
                    }
                },
                #endregion
            };

            // ACT
            var roads = RoadElementComposer.Compose(roadElements).ToArray();

            // ASSERT
            Assert.AreEqual(3, roads.Length);
            Assert.AreEqual(2, roads[0].Count);
            Assert.AreEqual(1, roads[1].Count);
            Assert.AreEqual(2, roads[2].Count);
        }
    }
}
