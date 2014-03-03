using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Explorer;
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

            var center = new GeoCoordinate(52.529814, 13.388015);
            var componentRoot = new ComponentRoot(@"Config/app.config");

            var floorBuilder = componentRoot.Container.Resolve<ITerrainBuilder>();

            floorBuilder.Build(new Tile(new MapScene(), center, new Vector2(0, 0), 10));

            Debug.Log("Generate Terrain: Done");
        }

        [MenuItem("OSM/Generate Single Building")]
        static void BuildSingle()
        {
            Debug.Log("Generate Single Building..");
            var b1 = new Building()
            {
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

            var center = new GeoCoordinate(52.529814, 13.388015);

            var componentRoot = new ComponentRoot(@"Config/app.config");

            var floorBuilder = componentRoot.Container.Resolve<ITerrainBuilder>();

            var floor = floorBuilder.Build(new Tile(
                    null, 
                    new GeoCoordinate(52.529814, 13.388015),
                    new Vector2(0, 0),
                    50));

            var visitor = componentRoot.Container.Resolve<ISceneModelVisitor>("building");
            visitor.VisitBuilding(center, floor, b1);

            Debug.Log("Generate Single Building: Done");
        }

        static void BuildCustomStyled()
        {
            /*var visitor = new BuildingModelVisitor(
                new PolygonMeshBuilder(),
                new DefaultMeshRenderer(Shader.Find("Bumped Diffuse"), Color.green),
                0,
                1.5f);

            visitor.VisitBuilding(center, null, b1);*/
        }

        [MenuItem("OSM/Generate Berlin Small Part")]
        static void BuildSmallPart()
        {
            Debug.Log("Generate Berlin Small Part..");

            var componentRoot = new ComponentRoot(@"Config/app.config");
            componentRoot.RunGame(new GeoCoordinate(51.26371, 4.7854));

            Debug.Log("Generate Berlin Small Part: Done");
        }
    }
}
