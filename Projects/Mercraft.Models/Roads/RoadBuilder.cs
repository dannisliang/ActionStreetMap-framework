using System.Linq;
using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Models.Roads
{
    public class RoadBuilder
    {
        private const double SimplificationTolerance = 2;

        public GameObject Build(RoadSettings roadSettings)
        {
            var gameObject = roadSettings.TargetObject.GetComponent<GameObject>();
            gameObject.AddComponent(typeof(MeshFilter));
            gameObject.AddComponent<MeshRenderer>().material = Resources.Load<Material>(@"Materials/RoadMaterial");

            var pathScript = new PathScript();

            pathScript.NewPath(gameObject, roadSettings.TerrainObject.GetComponent<GameObject>());
            pathScript.PathWidth = roadSettings.Width;

            var points = Geometry
                .DouglasPeuckerReduction(roadSettings.Points, SimplificationTolerance)
                .Select(p => new Vector3(p.X, 0, p.Y));

            foreach (var point in points)
            {
                pathScript.CreatePathNode(point);
            }
         
            pathScript.FinalizePath();
            return gameObject;
        }
    }
}
