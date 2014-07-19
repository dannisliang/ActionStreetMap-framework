using System.Linq;
using Mercraft.Core.Unity;
using Mercraft.Core.World.Roads;
using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Models.Roads
{
    public class RoadBuilder
    {
        private const double SimplificationTolerance = 10;

        public GameObject Build(Road road, IGameObject terrainGameObject)
        {
            var gameObject = road.GameObject.GetComponent<GameObject>();
            gameObject.AddComponent(typeof(MeshFilter));
            // TODO resolve material
            gameObject.AddComponent<MeshRenderer>().material = Resources.Load<Material>(@"Materials/RoadMaterial");

            var pathScript = new PathScript();
            pathScript.NewPath(gameObject, terrainGameObject.GetComponent<GameObject>());

            foreach (var roadElement in road.Elements)
            {
                var points = Geometry
                 .DouglasPeuckerReduction(roadElement.Points, SimplificationTolerance)
                 .Select(p => new Vector3(p.X, 0, p.Y));
                foreach (var point in points)
                {
                    pathScript.AddNode(point, roadElement.Width);
                }
            }
         
            pathScript.FinalizePath();
            return gameObject;
        }
    }
}
