using System;
using Mercraft.Core.Scene.Models;
using UnityEngine;

namespace Mercraft.Core.Interactions
{
    public interface IBehaviour
    {
        string Name { get; }
        void Apply(GameObject gameObject);
    }
}
