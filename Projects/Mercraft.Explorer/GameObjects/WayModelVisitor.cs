using Mercraft.Core;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using UnityEngine;

namespace Mercraft.Explorer.GameObjects
{
    public class WayModelVisitor: SceneModelVisitor
    {
        public override GameObject VisitWay(GeoCoordinate center, GameObject parent, Rule rule, Way way)
        {
            return null;
        }
    }
}
