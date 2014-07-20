using System.Linq;
using Mercraft.Core.Geometry;
using Mercraft.Core.Unity;
using Mercraft.Core.World.Roads;
using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Models.Roads
{
    public class RoadBuilder
    {
        private const double SimplificationTolerance = 10;

        /// <summary>
        /// Height of road. So far, we support only flat surfaces
        /// </summary>
        private const float Height = 0.1f;

        public GameObject Build(Road road, IGameObject terrainGameObject)
        {
            var gameObject = road.GameObject.GetComponent<GameObject>();
            gameObject.AddComponent(typeof(MeshFilter));
            // TODO resolve material
            gameObject.AddComponent<MeshRenderer>().material = Resources.Load<Material>(@"Materials/RoadMaterial");

            var pathScript = new PathScript();
            // NOTE This is just workaround to avoid flickering of overlapping roads
            pathScript.HeightY = Height + Random.Range(0.001f, 0.01f);
            pathScript.NewPath(gameObject, terrainGameObject.GetComponent<GameObject>());

            foreach (var roadElement in road.Elements)
            {
                var points = DouglasPeuckerReduction
                 .Reduce(roadElement.Points, SimplificationTolerance)
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
