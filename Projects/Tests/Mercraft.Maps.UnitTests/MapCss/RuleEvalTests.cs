using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Mercraft.Core;
using Mercraft.Core.MapCss;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Explorer;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Visitors;
using NUnit.Framework;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.MapCss
{
    [TestFixture]
    class RuleEvalTests
    {
        [Test]
        public void CanUseCanvas()
        {
            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile);
            var stylesheet = provider.Get();
            var canvas = new Canvas();

            var rule = stylesheet.GetRule(canvas);
            var material = rule.Evaluate<string>(canvas, "material");

            Assert.AreEqual("Terrain", material);

        }

        [Test]
        public void CanMergeDeclarations()
        {
            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile);
            var stylesheet = provider.Get();

            var area = new Area()
            {
                Id = 1,
                Points = new GeoCoordinate[]
                {
                    new GeoCoordinate(52.5212186,13.4096926),
			        new GeoCoordinate(52.5210184,13.4097473),
			        new GeoCoordinate(52.5209891,13.4097538),
			        new GeoCoordinate(52.5209766,13.4098037),
                },
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building","residential"),
                    new KeyValuePair<string, string>("building:shape","sphere"),
                    new KeyValuePair<string, string>("min_height","100"),
                    new KeyValuePair<string, string>("building:levels","5"),
                }
            };

            var rule = stylesheet.GetRule(area);

            Assert.IsTrue(rule.IsApplicable, "Unable to get declarations!");

            Assert.AreEqual("sphere", rule.Evaluate<string>(area, "builder"), "Unable to merge declarations!");
            Assert.AreEqual(100, rule.Evaluate<float>(area, "min_height"), "Unable to eval min_height from tag!");
            Assert.AreEqual(new Color32(188, 169, 169, 1), rule.Evaluate<Color32>(area, "fill-color"), "Unable to merge declarations!");
            Assert.AreEqual("solid", rule.Evaluate<string>(area, "behaviour"), "First rule isn't applied!");
            Assert.AreEqual("Concrete_Patterned", rule.Evaluate<string>(area, "material"), "First rule isn't applied!");
            Assert.AreEqual(15, rule.Evaluate<float>(area, "height"), "Unable to eval height from building:levels!");
        }


        [Test]
        public void CanGGGG()
        {

            Area area1 = null;
            Area area2 = null;
            using (Stream stream = new FileInfo(TestHelper.TestBigPbfFilePath).OpenRead())
            {
                var dataSource = new PbfElementSource(stream);

                var bbox = BoundingBox.CreateBoundingBox(new GeoCoordinate(52.520833, 13.409403), 100);

                var scene = new MapScene();

                var elementManager = new ElementManager();

                elementManager.VisitBoundingBox(bbox, dataSource, new WayVisitor(scene));

                foreach (var a in scene.Areas)
                {
                    if (a.Id == 19046101)
                    {
                        area1 = a;
                    }

                    if (a.Id == 26037206)
                    {
                        area2 = a;
                    }

                    if(area1 != null && area2 != null)
                    {
                        break;
                    }
                }
            }

            Container container = new Container();
            var center = new GeoCoordinate(52.529814, 13.388015);
            var componentRoot = new GameRunner(container, new ConfigSettings(TestHelper.ConfigRootFile));
            var stylesheet = container.Resolve<IStylesheetProvider>().Get();

            var rule1 = stylesheet.GetRule(area1);
            var rule2 = stylesheet.GetRule(area2);


           Assert.AreEqual(12f, rule2.GetHeight(area2));


        }

        [Test]
        public void CanUseSimpleEvaluate()
        {
            var model = new Area()
            {
                Id = 1,
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building:levels", "5")
                }
            };

            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile);
            var stylesheet = provider.Get();

            var evalDeclaration = stylesheet.Styles[3].Declarations[0];

            var evalResult = evalDeclaration.Evaluator.Walk<float>(model);

            Assert.AreEqual(15, evalResult);

        }

        [Test]
        public void CanGetMissing()
        {
            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile);
            var stylesheet = provider.Get();

            var area = new Area()
            {
                Id = 1,
                Points = new GeoCoordinate[0],
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building","residential"),
                }
            };
            var rule = stylesheet.GetRule(area);

            Assert.AreEqual(0, rule.GetLevels(area));

        }

        [Test]
        public void CanGetColor()
        {
            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile);
            var stylesheet = provider.Get();

            var park = new Area()
            {
                Id = 1,
                Points = new GeoCoordinate[0],
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building","yes"),
                }
            };
            var rule = stylesheet.GetRule(park);

            Assert.AreEqual(new Color32(188, 169, 169, 1), rule.GetFillColor(park, Color.green));
        }


        [Test]
        public void CanUseClosed()
        {
            var provider = new StylesheetProvider(TestHelper.TestBaseMapcssFile);
            var stylesheet = provider.Get();

            var closedWay = new Way()
            {
                Points = new[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 0),
                },
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("barrier", "yes")
                }
            };

            Assert.IsTrue(stylesheet.GetRule(closedWay).IsApplicable);


            var openWay = new Way()
            {
                Points = new[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 1),
                },
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("barrier", "yes")
                }
            };

            Assert.IsFalse(stylesheet.GetRule(openWay).IsApplicable);

        }
    }
}
