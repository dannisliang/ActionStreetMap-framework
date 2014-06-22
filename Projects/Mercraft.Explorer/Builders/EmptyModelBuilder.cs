using System;
using Mercraft.Core;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Infrastructure.Dependencies;

namespace Mercraft.Explorer.Builders
{
    public class EmptyModelBuilder : ModelBuilder
    {
        [Dependency]
        public EmptyModelBuilder(IGameObjectFactory goFactory)
            : base(goFactory)
        {
        }

        public override IGameObject BuildArea(GeoCoordinate center, Rule rule, Area area)
        {
            base.BuildArea(center, rule, area);
            return _goFactory.CreateNew(String.Format("Empty {0}", area));
        }

        public override IGameObject BuildWay(GeoCoordinate center, Rule rule, Way way)
        {
            base.BuildWay(center, rule, way);
            return _goFactory.CreateNew(String.Format("Empty {0}", way));
        }
    }
}