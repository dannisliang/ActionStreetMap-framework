using System;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.Scene.World.Buildings;
using Mercraft.Core.Unity;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Geometry.Polygons;
using UnityEngine;

namespace Mercraft.Models.Buildings.Roofs
{
    /// <summary>
    ///     Builds dome roof.
    /// </summary>
    public class DomeRoofBuilder: IRoofBuilder
    {
        private readonly IGameObjectFactory _gameObjectFactory;

        /// <inheritdoc />
        public string Name { get { return "dome"; } }

        /// <summary>
        ///     Creates DomeRoofBuilder
        /// </summary>
        [Dependency]
        public DomeRoofBuilder(IGameObjectFactory gameObjectFactory)
        {
            _gameObjectFactory = gameObjectFactory;
        }

        /// <inheritdoc />
        public bool CanBuild(Building building)
        {
            // we should use this builder only in case of dome type defined explicitly
            return building.RoofType == Name;
        }

        /// <inheritdoc />
        public MeshData Build(Building building, BuildingStyle style)
        {
            IGameObject gameObjectWrapper = _gameObjectFactory.CreatePrimitive(Name, UnityPrimitiveType.Sphere);

            var tuple = CircleHelper.GetCircle(building.Footprint);

            var diameter = tuple.Item1;
            var center = tuple.Item2;

            if(Math.Abs(diameter) < float.Epsilon)
                throw new ArgumentException("Dome roof builder expects roof-height set to non-zero positive value");

            // if offset is zero, than we will use hemisphere
            float offset = 0;
            if (building.RoofHeight > 0)
                offset = building.RoofHeight - diameter/2;

            center.SetElevation(building.Elevation + building.Height + building.MinHeight + offset);

            ProcessObject(gameObjectWrapper, center, diameter);

            return new MeshData()
            {
                MaterialKey = style.Roof.Path,
                GameObject = gameObjectWrapper
            };
        }

        /// <summary>
        ///     Sets Unity specific data.
        /// </summary>
        /// <param name="gameObjectWrapper">GameObject wrapper.</param>
        /// <param name="center">Sphere center.</param>
        /// <param name="diameter">Diameter.</param>
        protected virtual void ProcessObject(IGameObject gameObjectWrapper, MapPoint center, float diameter)
        {
            var sphere = gameObjectWrapper.GetComponent<GameObject>();
            sphere.transform.localScale = new Vector3(diameter, diameter, diameter);
            sphere.transform.position = new Vector3(center.X, center.Elevation, center.Y);
        }
    }
}
