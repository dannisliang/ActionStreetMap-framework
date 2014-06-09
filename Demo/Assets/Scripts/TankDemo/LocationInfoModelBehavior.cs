using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Interactions;
using Mercraft.Maps.Osm.Helpers;
using UnityEngine;

using LocationInfo = Mercraft.Core.LocationInfo;

namespace Assets.Scripts.TankDemo
{
    public class LocationInfoModelBehavior : MonoBehaviour, IModelBehaviour
    {
        private LocationInfo _locationInfo;

        public string Name { get; private set; }

        public void Apply(IGameObject go, Model model)
        {
            _locationInfo = LocationInfoExtractor.Extract(model.Tags);
        }

        // TODO show to user information from LocationInfo object
    }
}