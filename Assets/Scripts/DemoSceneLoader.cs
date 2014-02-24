
using System.Collections.Generic;
using System.IO;
using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Visitors;
using Mercraft.Models;
using Mercraft.Models.Scene;
using Mercraft.Scene.Builders;
using UnityEditor;
using UnityEngine;

namespace Mercraft.Scene
{
    class DemoSceneLoader
    {
        [MenuItem("OSM/Generate Single Building")]
        static void BuildSingle()
        {
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
            var builder = new BuildingBuilder(center);

            Object.DestroyImmediate(GameObject.Find("Building1"));
            builder.Build("Building1", b1);
        }

        [MenuItem("OSM/Generate Berlin Small Part")]
        static void Build()
        {
            Debug.Log("Start to create building..");

            //var file = @"c:\Users\Ilya.Builuk\Documents\Source\mercraft\Tests\TestAssets\kempen.osm.pbf";
            //var center = new GeoCoordinate(51.26371, 4.7854);

            var file = @".\Projects\Tests\TestAssets\berlin_house.osm.xml";
            var center = new GeoCoordinate(52.529814, 13.388015);

            using (Stream stream = new FileInfo(file).OpenRead())
            {
                var dataSource = MemoryDataSource.CreateFromXmlStream(stream);

                var bbox = BoundingBox.CreateBoundingBox(center, 1);

                var scene = new CountableScene();

                var elementManager = new ElementManager(new BuildingVisitor(scene));

                elementManager.VisitBoundingBox(dataSource, bbox);

                var builder = new BuildingBuilder(center);

                for (int i = 0; i < scene.Buildings.Count; i++)
                {
                    var building = scene.Buildings[i];
                    var name = "Building" + i;
                    Object.DestroyImmediate(GameObject.Find(name));
                    builder.Build(name, building);
                }
            }
            Debug.Log("Done");

        }
    }
}
