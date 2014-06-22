using System;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;

namespace Mercraft.Core.Scene
{
    public interface IModelBuilder
    {
        string Name { get; }
        IGameObject BuildArea(GeoCoordinate center, Rule rule, Area area);
        IGameObject BuildWay(GeoCoordinate center,  Rule rule, Way way);
    }

    public class ModelBuilder : IModelBuilder, IConfigurable
    {
        private const string NameKey = "@name";
        public string Name { get; private set; }

        [Dependency]
        protected ITrace Trace { get; set; }

        protected readonly IGameObjectFactory _goFactory;

        [Dependency]
        public ModelBuilder(IGameObjectFactory goFactory)
        {
            _goFactory = goFactory;
        }

        public virtual IGameObject BuildArea(GeoCoordinate center, Rule rule, Area area)
        {
            Trace.Normal(String.Format("{0}: building area {1}", Name, area.Id));
            return null;
        }

        public virtual IGameObject BuildWay(GeoCoordinate center, Rule rule, Way way)
        {
            Trace.Normal(String.Format("{0}: building way {1}", Name, way.Id));
            return null;
        }

        public virtual void Configure(IConfigSection configSection)
        {
            Name = configSection.GetString(NameKey);
        }
    }
}
