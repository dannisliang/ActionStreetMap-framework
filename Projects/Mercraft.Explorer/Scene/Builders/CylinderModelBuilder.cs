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
    public class CylinderModelBuilder : ModelBuilder
    {
        private readonly IResourceProvider _resourceProvider;

        public override string Name
        {
            get { return "cylinder"; }
        }

        [Dependency]
        public CylinderModelBuilder(WorldManager worldManager, IGameObjectFactory gameObjectFactory, 
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
            var cylinderCenter = circle.Item2;

            var height = rule.GetHeight();
            var minHeight = rule.GetMinHeight();

            var actualHeight = (height - minHeight) / 2;

            var gameObjectWrapper = GameObjectFactory.CreatePrimitive(String.Format("Cylinder {0}", area),
                UnityPrimitiveType.Cylinder);

            WorldManager.AddModel(area.Id);

            return BuildCylinder(gameObjectWrapper, rule, area, cylinderCenter, diameter, actualHeight, minHeight);
        }

        protected virtual IGameObject BuildCylinder(IGameObject gameObjectWrapper, Rule rule, Model model,
            MapPoint cylinderCenter, float diameter, float actualHeight, float minHeight)
        {
            var cylinder = gameObjectWrapper.GetComponent<GameObject>();

            cylinder.transform.localScale = new Vector3(diameter, actualHeight, diameter);
            cylinder.transform.position = new Vector3(cylinderCenter.X, minHeight + actualHeight, cylinderCenter.Y);

            cylinder.AddComponent<MeshRenderer>();
            cylinder.renderer.material = rule.GetMaterial(_resourceProvider);
            cylinder.renderer.material.color = rule.GetFillUnityColor();

            return gameObjectWrapper;
        }
    }
}