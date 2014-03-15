using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Explorer;
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

            var canvasVisitor = container.Resolve<ISceneModelVisitor>("canvas");
            var canvas = new Canvas()
            {
                Tile = new Tile(
                    null,
                    new GeoCoordinate(52.529814, 13.388015),
                    new Vector2(0, 0),
                    50)
            };

            var canvasRule = stylesheet.GetRule(canvas);
            var canvasGameObject = canvasVisitor.VisitCanvas(new GeoCoordinate(52.529814, 13.388015), null, canvasRule, canvas);



            Debug.Log("Generate Terrain: Done");
        }

        [MenuItem("OSM/Generate Single Building")]
        static void BuildSingle()
        {
            Debug.Log("Generate Single Building..");
            var b1 = new Area()
            {
                Tags = new Collection<KeyValuePair<string, string>>()
                {
                  new KeyValuePair<string, string>("building", "residential")  
                },
                Points = new List<GeoCoordinate>()
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

            var canvasVisitor = container.Resolve<ISceneModelVisitor>("canvas");
            var canvas = new Canvas()
            {
                Tile = new Tile(
                    null,
                    new GeoCoordinate(52.529814, 13.388015),
                    new Vector2(0, 0),
                    50)
            };

            var canvasRule = stylesheet.GetRule(canvas);
            var canvasGameObject =  canvasVisitor.VisitCanvas(new GeoCoordinate(52.529814, 13.388015), null, canvasRule, canvas);

            var visitor = container.Resolve<ISceneModelVisitor>("building");
            
            var rule = stylesheet.GetRule(b1);

            Debug.Log(rule);


            var selectors = rule.Selectors.ToList();
            var descirptors = rule.Declarations.ToList();


            visitor.VisitArea(center, canvasGameObject, rule, b1);

            Debug.Log("Generate Single Building: Done");          

        }

        [MenuItem("OSM/Generate Berlin Small Part")]
        static void BuildSmallPart()
        {
            Debug.Log("Generate Berlin Small Part..");

            var componentRoot = new GameRunner(@"Config/app.config");
            componentRoot.RunGame(new GeoCoordinate(51.26371, 4.7854));

            Debug.Log("Generate Berlin Small Part: Done");

            componentRoot.OnMapPositionChanged(new Vector2(0,0));
        }
    }
}
