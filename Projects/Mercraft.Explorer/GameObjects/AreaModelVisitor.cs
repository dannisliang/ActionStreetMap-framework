using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Explorer.Builders;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;
using UnityEngine;

namespace Mercraft.Explorer.GameObjects
{
    public class AreaModelVisitor: SceneModelVisitor
    {
        private readonly IEnumerable<IModelBuilder> _builders; 

        [Dependency]
        public AreaModelVisitor(IEnumerable<IModelBuilder> builders)
        {
            _builders = builders;
        }

        #region ISceneModelVisitor implementation

        public override GameObject VisitArea(GeoCoordinate center, GameObject parent, Rule rule, Area area)
        {
            var gameObject = new GameObject();
            gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();
            gameObject.AddComponent<MeshCollider>();
            gameObject.renderer.material = rule.GetMaterial(area); 

            rule.GetModelBuilder(area, _builders)
                .BuildArea(center, gameObject, rule, area);

            gameObject.transform.parent = parent.transform;

            return gameObject;
        }

        #endregion

    }
}
