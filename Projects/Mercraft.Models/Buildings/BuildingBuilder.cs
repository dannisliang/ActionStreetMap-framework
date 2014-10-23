using System;
using Mercraft.Core.Elevation;
using Mercraft.Core.Unity;
using Mercraft.Core.World.Buildings;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Buildings.Roofs;
using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Models.Buildings
{
    /// <summary>
    ///     Defines building builder logic.
    /// </summary>
    public interface IBuildingBuilder
    {
        /// <summary>
        ///     Builds building.
        /// </summary>
        /// <param name="heightMap">Heightmap.</param>
        /// <param name="building">Building.</param>
        /// <param name="style">Style.</param>
        void Build(HeightMap heightMap, Building building, BuildingStyle style);
    }

    /// <summary>
    ///     Default building builder.
    /// </summary>
    public class BuildingBuilder : IBuildingBuilder
    {
        private readonly IResourceProvider _resourceProvider;

        /// <summary>
        ///     Creates BuildingBuilder.
        /// </summary>
        /// <param name="resourceProvider">Resource provider.</param>
        [Dependency]
        public BuildingBuilder(IResourceProvider resourceProvider)
        {
            _resourceProvider = resourceProvider;
        }

        /// <inheritdoc />
        public void Build(HeightMap heightMap, Building building, BuildingStyle style)
        {
            var facadeMeshData = style.Facade.Builders[RandomHelper
                .GetIndex(building.Id, style.Facade.Builders.Length)]
                .Build(building, style);

            var roofMeshData = GetRoofBuilder(building, style.Roof.Builders)
               .Build(building, style);

            // NOTE use different gameObject only to support different materials
            AttachChildGameObject(building.GameObject, "facade", facadeMeshData);
            AttachChildGameObject(building.GameObject, "roof", roofMeshData);
        }

        /// <summary>
        ///     Process unity's game object
        /// </summary>
        protected virtual void AttachChildGameObject(IGameObject parent, string name, MeshData meshData)
        {
            var gameObject = new GameObject(name);
            gameObject.isStatic = true;
            gameObject.transform.parent = parent.GetComponent<GameObject>().transform;

            var mesh = new Mesh();
            mesh.vertices = meshData.Vertices;
            mesh.triangles = meshData.Triangles;
            mesh.uv = meshData.UV;
            mesh.RecalculateNormals();

            gameObject.AddComponent<MeshFilter>().mesh = mesh;
            gameObject.AddComponent<MeshCollider>();

            gameObject.AddComponent<MeshRenderer>().sharedMaterial = _resourceProvider
                .GetMatertial(meshData.MaterialKey);
        }

        private IRoofBuilder GetRoofBuilder(Building building, IRoofBuilder[] roofBuildings)
        {
            // for most of buildings, roof type isn't defined, but we want to use different types
            // however, we have to check whether we can build roof using random roof builder

            // contains true for index of roof builder if it can build roof for given building
            bool[] ids = new bool[roofBuildings.Length];

            int count = 0;
            for (int i = 0; i < roofBuildings.Length; i++)
            {
                if (roofBuildings[i].CanBuild(building))
                {
                    ids[i] = true;
                    count++;
                }
            }

            if (count == 0)
                throw new InvalidOperationException(String.Format(Strings.CannotFindRoofBuilder, building.Address));

            // however, we don't want to use first occurrence, use building Id as seed
            int index = 0;
            var seed = building.Id % count;
            while (true)
            {
                if (ids[index] && index == seed)
                    return roofBuildings[index];
                index++;
            }
        }
    }
}
