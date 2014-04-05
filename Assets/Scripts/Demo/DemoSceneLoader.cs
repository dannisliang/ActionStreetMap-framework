using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Assets.Scripts.Console;
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
        static IContainer GetContainer()
        {
            var consoleGameObject = new GameObject("_DebugConsole_");
            var debugConsole = consoleGameObject.AddComponent<DebugConsole>();
            var container = new Container();
            return container.RegisterInstance(debugConsole);
        }

        [MenuItem("OSM/Generate Terrain")]
        static void BuildFloor()
        {
            Debug.Log("Generate Terrain..");

            var container = GetContainer();
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

            var container = GetContainer();
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


        [MenuItem("OSM/Test Area")]
        static void BuildTestArea()
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

            var container = GetContainer();
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
            var b1 = new Way()
            {
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                  new KeyValuePair<string, string>("addr:housename", "Nokia"),
                },
                Points = new GeoCoordinate[]
                {
			        new GeoCoordinate(52.5304687,13.3848508),
                    new GeoCoordinate(52.5305881,13.3847611),
                    new GeoCoordinate(52.5306392,13.3849447),
                    new GeoCoordinate(52.5307996,13.3848242),
                    new GeoCoordinate(52.5307398,13.3846088),
                    new GeoCoordinate(52.5308306,13.3845405),
                    new GeoCoordinate(52.530897,13.3847792),
                    new GeoCoordinate(52.5309027,13.3847749),
                    new GeoCoordinate(52.5310071,13.3851909),
                    new GeoCoordinate(52.5310004,13.3851959),
                    new GeoCoordinate(52.5310746,13.3854629),
                    new GeoCoordinate(52.5309697,13.3855417),
                    new GeoCoordinate(52.5309201,13.3853632),
                    new GeoCoordinate(52.5307745,13.3854726),
                    new GeoCoordinate(52.5308213,13.3856409),
                    new GeoCoordinate(52.5307138,13.3857216),
                    new GeoCoordinate(52.5306414,13.3854611),
                    new GeoCoordinate(52.5306531,13.3854523),
                    new GeoCoordinate(52.5305942,13.3852405),
                    new GeoCoordinate(52.53058,13.3852511),
                    new GeoCoordinate(52.5304687,13.3848508),
                }
            };


            var container = GetContainer();
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

            visitor.FromWay(center, canvasGameObject, rule, b1);

            Debug.Log("Generate Single Building: Done");          

        }

        [MenuItem("OSM/Generate Berlin Small Part")]
        static void BuildSmallPart()
        {
            Debug.Log("Generate Berlin Small Part..");

            var container = GetContainer();
            var componentRoot = new GameRunner(container, @"Config/app.config");

            //componentRoot.RunGame(new GeoCoordinate(52.529814, 13.388015)); // home
            //componentRoot.RunGame(new GeoCoordinate(52.520833, 13.409403)); // teletower
            componentRoot.RunGame(new GeoCoordinate(52.531036, 13.384866)); // invaliden
            
            Debug.Log("Generate Berlin Small Part: Done");

            componentRoot.OnMapPositionChanged(new Vector2(0,0));
        }
    }
}
