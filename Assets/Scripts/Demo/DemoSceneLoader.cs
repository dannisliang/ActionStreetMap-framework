using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Explorer;
//using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Demo
{
    class DemoSceneLoader
    {
        //[MenuItem("OSM/Generate Floor")]
        static void BuildFloor()
        {
            Debug.Log("Generate Floor..");

            var center = new GeoCoordinate(52.529814, 13.388015);
            var componentRoot = new ComponentRoot(@"Config/app.config");

            var floorBuilder = componentRoot.Container.Resolve<IFloorBuilder>();

            floorBuilder.Build(new Tile(new MapScene(), center, new Vector2(0, 0), 10));

            Debug.Log("Generate Floor: Done");
        }

        //[MenuItem("OSM/Generate Single Building")]
        static void BuildSingle()
        {
            Debug.Log("Generate Single Building..");
            var b1 = new Building()
            {
                Points = new List<GeoCoordinate>()
                {
                    new GeoCoordinate(52.5302492,13.3868234),
                    new GeoCoordinate(52.5304374,13.3866118),
                    new GeoCoordinate(52.5304738,13.3866946),
                    new GeoCoordinate(52.530385,13.3868148),
                    new GeoCoordinate(52.5304272,13.3869328),
                    new GeoCoordinate(52.5303328,13.3870465),
                    new GeoCoordinate(52.5303093,13.3869838),
                }
            };

            var center = new GeoCoordinate(52.529814, 13.388015);
            /*var visitor = new BuildingModelVisitor(
                new PolygonMeshBuilder(),
                new DefaultMeshRenderer(Shader.Find("Bumped Diffuse"), Color.green),
                0,
                1.5f);

            visitor.VisitBuilding(center, null, b1);*/

            var componentRoot = new ComponentRoot(@"Config/app.config");
            var visitor = componentRoot.Container.Resolve<ISceneModelVisitor>("building");
            visitor.VisitBuilding(center, null, b1);

            Debug.Log("Generate Single Building: Done");
        }

        //[MenuItem("OSM/Generate Berlin Small Part")]
        static void BuildSmallPart()
        {
            Debug.Log("Generate Berlin Small Part..");

            var componentRoot = new ComponentRoot(@"Config/app.config");
            componentRoot.RunGame(new GeoCoordinate(51.26371, 4.7854));

            Debug.Log("Generate Berlin Small Part: Done");
        }
    }
}
