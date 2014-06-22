using System;
using System.Collections.Generic;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Infrastructure.Config;
using UnityEngine;

namespace Mercraft.Explorer.Interactions
{
    public class CompositeModelBehaviour : IModelBehaviour, IConfigurable
    {
        private const string NameKey = "@name";
        private const string BehaviourKey = "scripts/include";
        private const string BehaviourValue = "@type";

        private readonly List<Type> _behaviours = new List<Type>();

        public string Name { get; private set; }

        public void Apply(IGameObject gameObject, Model model)
        {
            foreach (var behaviour in _behaviours)
            {
                // NOTE: behavior should implement Unity's MonoBehavior
                var component = gameObject.GetComponent<GameObject>()
                    .AddComponent(behaviour);

                // NOTE inject gameObject and model in case of our IModelBehavior
                // which allows us to use Mercraft's Model inside Unity's MonoBehavior
                var modelBehaviour = component as IModelBehaviour;
                if (modelBehaviour != null)
                {
                    modelBehaviour.Apply(gameObject, model);
                }
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