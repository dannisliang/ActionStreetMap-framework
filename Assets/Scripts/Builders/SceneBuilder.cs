
using System.IO;

using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Visitors;
using Mercraft.Models;
using Mercraft.Models.Scene;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Mercraft.Scene.Builders
{
    //[ExecuteInEditMode]
    public class SceneBuilder: MonoBehaviour
    {
        void Start()
        {
            Build();
        }

        #region Test4

        public void Build()
        {
            Debug.Log("Start to create building..");

            //var file = @"c:\Users\Ilya.Builuk\Documents\Source\mercraft\Tests\TestAssets\kempen.osm.pbf";
            //var center = new MapPoint(51.26371, 4.7854);

            var file = @"c:\Users\Ilya.Builuk\Documents\Source\mercraft\Projects\Tests\TestAssets\berlin_house.osm.xml";
            var center = new MapPoint(52.529814, 13.388015);
            
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
                    builder.Build("Building"+i, building);
                }
            }
            Debug.Log("Done");

        }

        #endregion

    }
}
