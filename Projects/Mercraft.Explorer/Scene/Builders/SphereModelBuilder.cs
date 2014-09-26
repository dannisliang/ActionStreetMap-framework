using System;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Core.World;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Explorer.Scene.Builders
{
    public class SphereModelBuilder : ModelBuilder
    {
        private readonly IResourceProvider _resourceProvider;

        public override string Name
        {
            get { return "sphere"; }
        }

        [Dependency]
        public SphereModelBuilder(WorldManager worldManager, IGameObjectFactory gameObjectFactory, 
            IResourceProvider resourceProvider)
            : base(worldManager, gameObjectFactory)
        {
            _resourceProvider = resourceProvider;
        }

        public override IGameObject BuildArea(Tile tile, Rule rule, Area area)
        {
            base.BuildArea(tile, rule, area);

            if (WorldManager.Contains(area.Id))
                return null;

            var circle = CircleHelper.GetCircle(tile.RelativeNullPoint, area.Points);
            var diameter = circle.Item1;
            var sphereCenter = circle.Item2;
            var minHeight = rule.GetMinHeight();

            IGameObject gameObjectWrapper = GameObjectFactory.CreatePrimitive(String.Format("Sphere {0}", area),
                UnityPrimitiveType.Sphere);

            WorldManager.AddModel(area.Id);

            return BuildSphere(gameObjectWrapper, rule, area, sphereCenter, diameter, minHeight);
        }

        protected virtual IGameObject BuildSphere(IGameObject gameObjectWrapper, Rule rule, Model model, 
            MapPoint sphereCenter, float diameter, float minHeight)
        {
            var sphere = gameObjectWrapper.GetComponent<GameObject>();

            sphere.AddComponent<MeshRenderer>();
            sphere.renderer.material = rule.GetMaterial(_resourceProvider);
            sphere.renderer.material.color = rule.GetFillUnityColor();

            sphere.transform.localScale = new Vector3(diameter, diameter, diameter);
            sphere.transform.position = new Vector3(sphereCenter.X, minHeight + diameter/2, sphereCenter.Y);

            return gameObjectWrapper;
        }
    }
}