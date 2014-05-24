using System;
using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Core.Unity;
using Mercraft.Infrastructure.Config;
using UnityEngine;

namespace Mercraft.Explorer.Interactions
{
    public class CompositeModelBehaviour: IModelBehaviour, IConfigurable
    {
        private const string NameKey = "@name";
        private const string BehaviourKey = "scripts/include";
        private const string BehaviourValue = "@type";

        private readonly List<Type> _behaviours = new List<Type>();

        public string Name { get; private set; }

        public void Apply(IGameObject gameObject)
        {
            foreach (var behaviour in _behaviours)
            {
                gameObject.GetComponent<GameObject>().AddComponent(behaviour);
            }
        }

        public void Configure(IConfigSection configSection)
        {
            Name = configSection.GetString(NameKey);
            foreach (var behaviourConfig in configSection.GetSections(BehaviourKey))
            {
                _behaviours.Add(behaviourConfig.GetType(BehaviourValue));
            }
        }      
    }
}
