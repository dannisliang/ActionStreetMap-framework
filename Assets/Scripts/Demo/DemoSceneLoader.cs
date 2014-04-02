using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Assets.Scripts.TankDemo;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Explorer;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Demo
{
    class DemoSceneLoader
    {
        [MenuItem("OSM/Generate Terrain")]
        static void BuildFloor()
        {
            Debug.Log("Generate Terrain..");

            Container container = new Container();
            var center = new GeoCoordinate(52.529814, 13.388015);
            var componentRoot = new GameRunner(container, new ConfigSettings(@"Config/app.config"));
            var stylesheet = container.Resolve<IStylesheetProvider>().Get();

            var canvasVisitor = container.Resolve<IGameObjectBuilder>("canvas");
            var canvas = new Canvas()
            {
                Tile = new Tile(
                    null,
                    new GeoCoordinate(52.529814, 13.388015),
                    new Vector2(0, 0),
                    50)
            };

            var canvasRule = stylesheet.GetRule(canvas);
            var canvasGameObject = canvasVisitor.FromCanvas(new GeoCoordinate(52.529814, 13.388015), null, canvasRule, canvas);



            Debug.Log("Generate Terrain: Done");
        }

        [MenuItem("OSM/Generate Way")]
        private static void BuildWay()
        {
            var way = new Way()
            {
                Id = 1,
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("highway", "residential")
                },
                Points = new GeoCoordinate[]
                {
                    new GeoCoordinate(52.5353039, 13.3960696),
                    new GeoCoordinate(52.5352872, 13.3960372),
                    new GeoCoordinate(52.5352709, 13.3960171),
                    new GeoCoordinate(52.5352482, 13.3960001),
                    new GeoCoordinate(52.5352333, 13.3959944),
                    new GeoCoordinate(52.535208, 13.395993),
                    new GeoCoordinate(52.5351826, 13.3960021),
                    new GeoCoordinate(52.535158, 13.3960228),
                    new GeoCoordinate(52.535137, 13.396054),
                    new GeoCoordinate(52.5351211, 13.3960985),
                    new GeoCoordinate(52.5351146, 13.396146),
                    new GeoCoordinate(52.5351157, 13.3961973),
                    new GeoCoordinate(52.5351246, 13.3962394),
                    new GeoCoordinate(52.5351475, 13.3962889),
                    new GeoCoordinate(52.5351682, 13.3963129),
                    new GeoCoordinate(52.5351925, 13.3963282),
                    new GeoCoordinate(52.5352181, 13.3963331),
                }
            };

            Container container = new Container();
            var center = new GeoCoordinate(52.529814, 13.388015);
            var componentRoot = new GameRunner(container, new ConfigSettings(@"Config/app.config"));
            var stylesheet = container.Resolve<IStylesheetProvider>().Get();
            var rule = stylesheet.GetRule(way);

            var points = PolygonHelper.GetVerticies2D(center, way.Points);


            GameObject gameObject = new GameObject("line");
            var lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = rule.GetMaterial(way);
            lineRenderer.material.color = rule.GetFillColor(way);
            lineRenderer.SetVertexCount(points.Length);

            // var color32 = rule.GetFillColor(way, Color.red);
            //var color = new Color(color32.r, color32.g, color32.b, 1);
            //Debug.Log(color);
            //lineRenderer.SetColors(color, color);

            for (int i = 0; i < points.Length; i++)
            {
                lineRenderer.SetPosition(i, new Vector3(points[i].x, 0, points[i].y));
            }

            lineRenderer.SetWidth(2, 2);

        }

        [MenuItem("OSM/Generate Sphere")]
        private static void BuildSphereAndCylinder()
        {
            Area sphereArea = new Area()
            {
                Points = new GeoCoordinate[]
                {
                    new GeoCoordinate(52.5209077, 13.4092488),
                    new GeoCoordinate(52.5209278, 13.4092833),
                    new GeoCoordinate(52.5209422, 13.4093212),
                    new GeoCoordinate(52.520953, 13.4093712),
                    new GeoCoordinate(52.5209563, 13.4094166),
                    new GeoCoordinate(52.5209525, 13.4094729),
                    new GeoCoordinate(52.5209457, 13.4095069),
                    new GeoCoordinate(52.5209377, 13.4095331),
                    new GeoCoordinate(52.5209251, 13.4095629),
                    new GeoCoordinate(52.5209185, 13.4095751),
                    new GeoCoordinate(52.5209112, 13.4095867),
                    new GeoCoordinate(52.5209028, 13.4095981),
                    new GeoCoordinate(52.5208857, 13.4096164),
                    new GeoCoordinate(52.5208716, 13.4096275),
                    new GeoCoordinate(52.5208627, 13.4096329),
                    new GeoCoordinate(52.5208502, 13.4096386),
                    new GeoCoordinate(52.5208273, 13.4096439),
                    new GeoCoordinate(52.520798, 13.4096412),
                    new GeoCoordinate(52.5207724, 13.4096301),
                    new GeoCoordinate(52.5207491, 13.4096114),
                    new GeoCoordinate(52.5207327, 13.4095921),
                    new GeoCoordinate(52.5207164, 13.4095656),
                    new GeoCoordinate(52.5207011, 13.4095296),
                    new GeoCoordinate(52.520691, 13.4094922),
                    new GeoCoordinate(52.5206845, 13.4094424),
                    new GeoCoordinate(52.5206845, 13.4093974),
                    new GeoCoordinate(52.52069, 13.4093534),
                    new GeoCoordinate(52.5207022, 13.4093078),
                    new GeoCoordinate(52.5207195, 13.4092692),
                    new GeoCoordinate(52.5207363, 13.4092436),
                    new GeoCoordinate(52.5207551, 13.4092234),
                    new GeoCoordinate(52.5207807, 13.4092059),
                    new GeoCoordinate(52.5208092, 13.409197),
                    new GeoCoordinate(52.5208373, 13.4091981),
                    new GeoCoordinate(52.5208635, 13.409208),
                    new GeoCoordinate(52.5208862, 13.4092245),
                    new GeoCoordinate(52.5209077, 13.4092488),
                },
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("/building:shape","sphere"),
                    new KeyValuePair<string, string>("height","237"),
                    new KeyValuePair<string, string>("min_height","205"),
                    new KeyValuePair<string, string>("building","tower"),
                }
            };

            //building:shape:sphere,ele:40,height:237,man_made:tower,min_height:205

            //GenerateSphere(sphereArea);

            Area cylinderArea = new Area()
            {
                Points = new GeoCoordinate[]
                {
                    new GeoCoordinate(52.5207997, 13.4093869),
                    new GeoCoordinate(52.5208052, 13.4093789),
                    new GeoCoordinate(52.5208096, 13.4093751),
                    new GeoCoordinate(52.5208159, 13.4093723),
                    new GeoCoordinate(52.5208212, 13.4093719),
                    new GeoCoordinate(52.520826, 13.4093732),
                    new GeoCoordinate(52.5208297, 13.4093752),
                    new GeoCoordinate(52.5208339, 13.4093789),
                    new GeoCoordinate(52.5208377, 13.4093838),
                    new GeoCoordinate(52.5208417, 13.4093921),
                    new GeoCoordinate(52.5208433, 13.4093971),
                    new GeoCoordinate(52.5208446, 13.4094026),
                    new GeoCoordinate(52.5208455, 13.4094108),
                    new GeoCoordinate(52.5208456, 13.4094176),
                    new GeoCoordinate(52.5208451, 13.4094229),
                    new GeoCoordinate(52.5208441, 13.4094291),
                    new GeoCoordinate(52.5208425, 13.409435),
                    new GeoCoordinate(52.5208407, 13.4094397),
                    new GeoCoordinate(52.5208366, 13.4094472),
                    new GeoCoordinate(52.5208331, 13.4094513),
                    new GeoCoordinate(52.5208279, 13.4094553),
                    new GeoCoordinate(52.5208235, 13.409457),
                    new GeoCoordinate(52.5208168, 13.4094573),
                    new GeoCoordinate(52.5208102, 13.4094547),
                    new GeoCoordinate(52.5208043, 13.4094495),
                    new GeoCoordinate(52.5207983, 13.4094395),
                    new GeoCoordinate(52.5207957, 13.4094318),
                    new GeoCoordinate(52.5207944, 13.4094258),
                    new GeoCoordinate(52.5207935, 13.4094171),
                    new GeoCoordinate(52.5207937, 13.4094092),
                    new GeoCoordinate(52.5207945, 13.4094031),
                    new GeoCoordinate(52.5207968, 13.4093938),
                    new GeoCoordinate(52.5207997, 13.4093869),
                }
            };

            GenerateCylinder(cylinderArea);

        }

        private static void GenerateSphere(Area area)
        {
            Container container = new Container();
            var center = new GeoCoordinate(52.529814, 13.388015);
            var componentRoot = new GameRunner(container, new ConfigSettings(@"Config/app.config"));
            var stylesheet = container.Resolve<IStylesheetProvider>().Get();

            var canvasVisitor = container.Resolve<IGameObjectBuilder>("canvas");
            var canvas = new Canvas()
            {
                Tile = new Tile(
                    null,
                    new GeoCoordinate(52.529814, 13.388015),
                    new Vector2(0, 0),
                    50)
            };

            var canvasRule = stylesheet.GetRule(canvas);
            var canvasGameObject = canvasVisitor.FromCanvas(new GeoCoordinate(52.520833, 13.409403), null, canvasRule, canvas);

            var visitor = container.Resolve<IGameObjectBuilder>("sphere");

            var rule = stylesheet.GetRule(area);

            visitor.FromArea(center, canvasGameObject, rule, area);

            Debug.Log("Generate Single Building: Done");          


           /* var minHeight = 205;
            var height = 237;

            var center = new GeoCoordinate(52.520833, 13.409403);

            //calculate radius
            var minLat = area.Points.Min(a => a.Latitude);
            var maxLat = area.Points.Max(a => a.Latitude);
            float diameter = (float) ((maxLat - minLat)/(180/Math.PI)*6378137);

            var minLon = area.Points.Min(a => a.Longitude);
            var maxLon = area.Points.Max(a => a.Longitude);

            float centerLat = (float)(minLat + (maxLat - minLat) / 2);
            float centerLon = (float)(minLon + (maxLon - minLon) / 2);
            var sphereCenter = GeoProjection.ToMapCoordinate(center,
                new GeoCoordinate(centerLat, centerLon));


            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(diameter, diameter, diameter);
            sphere.transform.position = new Vector3(sphereCenter.x, minHeight + diameter / 2, sphereCenter.y);*/
        }

        private static void GenerateCylinder(Area area)
        {
            var minHeight = 0;
            var height = 205;
            var center = new GeoCoordinate(52.520833, 13.409403);

            var circle = CircleHelper.GetCircle(center, area.Points);
            var diameter = circle.Item1;
            var cylinderCenter = circle.Item2;

            var actualHeight = (height - minHeight)/2;

            var cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.transform.localScale = new Vector3(diameter, actualHeight, diameter);
            cylinder.transform.position = new Vector3(cylinderCenter.x, actualHeight, cylinderCenter.y);

            /*var actualHeight = height - minHeight;

            //calculate radius
            var minLat = area.Points.Min(a => a.Latitude);
            var maxLat = area.Points.Max(a => a.Latitude);
            float diameter = (float)((maxLat - minLat) / (180 / Math.PI) * 6378137);

            var sphere = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            sphere.transform.localScale = new Vector3(diameter, actualHeight, diameter);
            sphere.transform.position = new Vector3(0, minHeight, 0);*/
        }

        [MenuItem("OSM/GGG")]
        static void BuildGGG()
        {
            var area = new Area()
            {
                Id = 1,
                Points = new GeoCoordinate[]
                {
                    new GeoCoordinate(52.5212186,13.4096926),
			        new GeoCoordinate(52.5210184,13.4097473),
			        new GeoCoordinate(52.5209891,13.4097538),
			        new GeoCoordinate(52.5209766,13.4098037),
                    new GeoCoordinate(52.5212186,13.4096926),
                },
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("building","roof"),
                    new KeyValuePair<string, string>("building:part","yes"),
                    new KeyValuePair<string, string>("level","1"),
                }
            };

            Container container = new Container();
            var center = new GeoCoordinate(52.529814, 13.388015);
            var componentRoot = new GameRunner(container, new ConfigSettings(@"Config/app.config"));
            var stylesheet = container.Resolve<IStylesheetProvider>().Get();

            var canvasVisitor = container.Resolve<IGameObjectBuilder>("canvas");
            var canvas = new Canvas()
            {
                Tile = new Tile(
                    null,
                    new GeoCoordinate(52.529814, 13.388015),
                    new Vector2(0, 0),
                    50)
            };

            var canvasRule = stylesheet.GetRule(canvas);
            var canvasGameObject = canvasVisitor.FromCanvas(new GeoCoordinate(52.529814, 13.388015), null, canvasRule, canvas);

            var visitor = container.Resolve<IGameObjectBuilder>();

            var rule = stylesheet.GetRule(area);

            visitor.FromArea(center, canvasGameObject, rule, area);

        }

        [MenuItem("OSM/Generate Single Building")]
        static void BuildSingle()
        {
            Debug.Log("Generate Single Building..");
            var b1 = new Area()
            {
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                  new KeyValuePair<string, string>("building", "residential"),
                  new KeyValuePair<string, string>("building:levels", "10")  
                },
                Points = new GeoCoordinate[]
                {
			        new GeoCoordinate(52.5295083,13.3889532),
			        new GeoCoordinate(52.5294599,13.3887466),
			        new GeoCoordinate(52.5293253,13.3888356),
			        new GeoCoordinate(52.529354,13.3889638),
			        new GeoCoordinate(52.5292772,13.3890143),
			        new GeoCoordinate(52.529244,13.3888741),
			        new GeoCoordinate(52.5291502,13.3889361),
			        new GeoCoordinate(52.5291819,13.389071),
			        new GeoCoordinate(52.5291244,13.3891088),
			        new GeoCoordinate(52.5291505,13.3891865),
			        new GeoCoordinate(52.5295083,13.3889532),
                }
            };


            Container container = new Container();
            var center = new GeoCoordinate(52.529814, 13.388015);
            var componentRoot = new GameRunner(container, new ConfigSettings(@"Config/app.config"));
            var stylesheet = container.Resolve<IStylesheetProvider>().Get();

            var canvasVisitor = container.Resolve<IGameObjectBuilder>("canvas");
            var canvas = new Canvas()
            {
                Tile = new Tile(
                    null,
                    new GeoCoordinate(52.529814, 13.388015),
                    new Vector2(0, 0),
                    50)
            };

            var canvasRule = stylesheet.GetRule(canvas);
            var canvasGameObject =  canvasVisitor.FromCanvas(new GeoCoordinate(52.529814, 13.388015), null, canvasRule, canvas);

            var visitor = container.Resolve<IGameObjectBuilder>("solid");
            
            var rule = stylesheet.GetRule(b1);

            visitor.FromArea(center, canvasGameObject, rule, b1);

            Debug.Log("Generate Single Building: Done");          

        }

        [MenuItem("OSM/Generate Berlin Small Part")]
        static void BuildSmallPart()
        {
            Debug.Log("Generate Berlin Small Part..");

            var componentRoot = new GameRunner(@"Config/app.config");

            //componentRoot.RunGame(new GeoCoordinate(52.529814, 13.388015)); // home
            componentRoot.RunGame(new GeoCoordinate(52.520833, 13.409403)); // teletower

            Debug.Log("Generate Berlin Small Part: Done");

            componentRoot.OnMapPositionChanged(new Vector2(0,0));
        }
    }
}
