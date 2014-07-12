using System.Linq;
using UnityEngine;

namespace Mercraft.Models.Roads
{
    public class RoadBuilder
    {
        public GameObject Build(RoadSettings roadSettings, float[,,] alphamap)
        {
            var terrain = roadSettings.TerrainObject
                .GetComponent<UnityEngine.Terrain>();

            var gameObject = roadSettings.TargetObject.GetComponent<GameObject>();
            gameObject.AddComponent(typeof(MeshFilter));
            gameObject.AddComponent<MeshRenderer>().material = Resources.Load<Material>(@"Materials/RoadMaterial");

            var pathScript = new PathScript();

            pathScript.NewPath(gameObject, roadSettings.TerrainObject.GetComponent<GameObject>());
            pathScript.isRoad = true;
            pathScript.addNodeMode = false;
            pathScript.pathWidth = roadSettings.Width;

            // TODO Add offset for coordinates from (0,0)
            var points = roadSettings.Points.Select(p => new Vector3(p.X, 0, p.Y));

            foreach (var point in points)
            {
                pathScript.CreatePathNode(GetTerrainPosition(point, terrain.terrainData));
            }
         
            pathScript.FinalizePath(alphamap);
            return gameObject;
        }

      
        private Vector3 GetTerrainPosition(Vector3 point, TerrainData terrainData)
        {
            var returnCollision = point;

            returnCollision.x = Mathf.RoundToInt((returnCollision.x / terrainData.size.x) * terrainData.heightmapResolution);
            returnCollision.y = returnCollision.y / terrainData.size.y;
            returnCollision.z = Mathf.RoundToInt((returnCollision.z / terrainData.size.z) * terrainData.heightmapResolution);
            return returnCollision;
        }
    }
}
