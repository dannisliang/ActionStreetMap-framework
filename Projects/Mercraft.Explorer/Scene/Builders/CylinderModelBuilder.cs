using System;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Helpers;
using UnityEngine;

namespace Mercraft.Explorer.Scene.Builders
{
    /// <summary>
    ///     Provides logic to build cylinders.
    /// </summary>
    public class CylinderModelBuilder : ModelBuilder
    {
        /// <inheritdoc />
        public override string Name
        {
            get { return "cylinder"; }
        }

        /// <inheritdoc />
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

        /// <summary>
        ///     Process unity specific data.
        /// </summary>
        protected virtual IGameObject BuildCylinder(IGameObject gameObjectWrapper, Rule rule, Model model,
            MapPoint cylinderCenter, float diameter, float actualHeight, float minHeight)
        {
            var cylinder = gameObjectWrapper.GetComponent<GameObject>();

            cylinder.transform.localScale = new Vector3(diameter, actualHeight, diameter);
            cylinder.transform.position = new Vector3(cylinderCenter.X, minHeight + actualHeight, cylinderCenter.Y);

            cylinder.AddComponent<MeshRenderer>();
            cylinder.renderer.material = rule.GetMaterial(ResourceProvider);
            cylinder.renderer.material.color = rule.GetFillUnityColor();

            return gameObjectWrapper;
        }
    }
}