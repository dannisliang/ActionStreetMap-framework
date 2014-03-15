using Mercraft.Core;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using UnityEngine;

namespace Mercraft.Explorer.Builders
{
    public interface IModelBuilder<T> where T: Model
    {
        string Name { get; }
        void Build(GeoCoordinate center, GameObject gameObject,  Rule rule, T model);
    }
}
